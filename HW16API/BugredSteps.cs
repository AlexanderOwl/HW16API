using HW16API.Properties;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TechTalk.SpecFlow;

namespace HW16API
{
    [Binding]
    public class BugredSteps
    {

        RestClient client;       
        Dictionary<string, object> companyData = new Dictionary<string, object>();
        IRestResponse response;
        string userName;
        string email;       
        string companyName;
        string companyType;
        List<string> companyUsers;
        string time = DateTime.Now.ToString().Replace(":", ".").Replace(" ", "").Replace("/", "");
       
        //Background
        [Given(@"create of a new rest client with url (.*)")]
        public void GivenCreateOfANewRestClientWithUrl(string baseUrl)
        {
            client = new RestClient(baseUrl);
        }

        
        #region CreateCompany       
        [Given(@"company name (.*)")]
        public void GivenCompanyName(string name)
        {
            companyName = name + time;
            companyData.Add("company_name", companyName);
        }

        [Given(@"type of company (.*)")]
        public void GivenTypeOfCompany(string type)
        {
            companyType = type;
            companyData.Add("company_type", companyType);
        }

        [Given(@"users")]
        public void GivenUsers()
        {
            companyUsers = new List<string> { "possuum@mail.ru", "testmail1@mail.ru" };
            companyData.Add("company_users", companyUsers);
        }

        [Given(@"user email (.*)")]
        public void GivenOwnerEmailOfNewCompany(string ownerEmail)
        {
            email = ownerEmail;
            companyData.Add("email_owner", email);
        }

        [When(@"send request to (.*) with valid data")]
        public void WhenSendRequestToWithValidData(string urn)
        { 
            RestRequest request = new RestRequest(urn, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(companyData);
            response = client.Execute(request);
        }       

        [Then(@"company has been created")]
        public void ThenCompanyHasBeenCreated()
        {
            Assert.AreEqual("OK", response.StatusCode.ToString());
        }

        [Then(@"name of the company = name from request")]
        public void ThenNameOfTheCompanyNameFromRequest()
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            string actualResult = json["company"]["name"].ToString();
            Assert.AreEqual(companyName, actualResult);
        }

        [Then(@"type of the company = type from request")]
        public void ThenTypeOfTheCompanyTypeFromRequest()
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            string actualResult = json["company"]["type"].ToString();
            Assert.AreEqual(companyType, actualResult);
        }

        [Then(@"users of the company = users from request")]
        public void ThenUsersOfTheCompanyUsersFromRequest()
        {
            var temp = response.Content;
            JObject json = JObject.Parse(temp);
            string actualResult1 = json["company"]["users"][0]?.ToString();
            string actualResult2 = json["company"]["users"][1]?.ToString();
            Assert.AreEqual("possuum@mail.ru", actualResult1);
            Assert.AreEqual("testmail1@mail.ru", actualResult2);
        }
        #endregion 

        #region add avatar       
        string avatarEmail;
        string avatarPath;
        [Given(@"email (.*) and avatar path (.*)")]
        public void GivenEmailAndAvatarPath(string email, string path)
        {
            avatarEmail = email;
            avatarPath = path;
        }
        [When(@"add avatar post request to (.*)")]
        public void WhenAddAvatarPostRequestTo(string urn)
        {
            RestRequest request = new RestRequest(urn + avatarEmail, Method.POST);
            request.AddFile("avatar", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, avatarPath));
            response = client.Execute(request);
        }
        [Then(@"status code OK")]
        public void ThenStatusCodeOK()
        {
            Assert.AreEqual("OK", response.StatusCode.ToString());
        }
        #endregion

        #region delete avatar
        string deleteAvatarEmail;
        [Given(@"account (.*)")]
        public void GivenAccount(string email)
        {
            deleteAvatarEmail = email;
        }
        [When(@"delete avatar post request to (.*)")]
        public void WhenDeleteAvatarPostRequestTo(string urn)
        {
            RestRequest request = new RestRequest(urn + deleteAvatarEmail, Method.POST);
            response = client.Execute(request);
        }
        #endregion

        #region doRegister
        string pass;
        

        [Given(@"name of user (.*)")]
        public void GivenNameOfUser(string name)
        {
            userName = name + time;
        }

        [Given(@"new email of user")]
        public void GivenEmailOfUser()
        {  
            email = "QA_Alex"+time + "@gmail.com"; 
        }
         
        [Given(@"user password")]
        public void GivenUserPassword()
        {
            pass = time;
        }

        [When(@"send post request to do register to (.*)")]
        public void WhenSendPostRequestToDoRegisterTo(string urn)
        {
            Dictionary<string, string> requestData = new Dictionary<string, string>
            {
                {"name", userName },
                {"email", email },
                {"password", pass }
            };

            RestRequest request = new RestRequest(urn, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(requestData);
            response = client.Execute(request);
        }

        [Then(@"account successful created status code (.*)")]
        public void ThenAccountSuccessfulCreatedStatusCode(string p0)
        {
            Assert.AreEqual("OK", response.StatusCode.ToString());
        }

        JObject responseJson;
        [Then(@"response have name of user")]
        public void ThenResponseHaveNameOfUser()
        {
            responseJson = JObject.Parse(response.Content);
            Assert.AreEqual(userName, responseJson["name"]?.ToString());
        }

        [Then(@"response have email of user")]
        public void ThenResponseHaveEmailOfUser()
        {
            Assert.AreEqual(email, responseJson["email"]?.ToString());
        }

        [Then(@"response doesn't have a non-encrypted password")]
        public void ThenResponseDoesnTHaveANon_EncryptedPassword()
        {
            Assert.IsFalse(responseJson["password"]?.ToString().Contains(pass));
        }

        #endregion

        #region doRegisterExistEmail
        [Given(@"user password (.*)")]
        public void GivenUserPassword(string userPassword)
        {
            pass = userPassword;
        }

        [Then(@"response type is (.*)")]
        public void ThenResponseTypeIs(string p0)
        {

            responseJson = JObject.Parse(response.Content);
            Assert.AreEqual("error", responseJson["type"]?.ToString());
        }
        [Then(@"response contains (.*) and (.*)")]
        public void ThenResponseContainsAnd(string msg, string subject)
        {
            responseJson = JObject.Parse(response.Content);
            Assert.IsTrue(responseJson["message"]?.ToString().Contains(msg));
            Assert.IsTrue(responseJson["message"]?.ToString().Contains(subject));
        }
        #endregion

        #region doRegisterExistName     
        [Given(@"exist name of user")]
        public void GivenExistNameOfUser()
        {
            userName = "Cашенька";
        }
        #endregion


    }
}
