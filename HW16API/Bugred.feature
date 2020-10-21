Feature: Bugred
	In order to give tasks to an employee 
    As an employer 
    I want to create my own company

Background:
    Given create of a new rest client with url http://users.bugred.ru

@createNewCompany @ООО
Scenario: Create a new company ООО
  Given company name QA_Alex & co
  And type of company <type>
  And users
  And user email divohi1607@fazmail.net
  When send request to /tasks/rest/createcompany with valid data
  Then company has been created  
  Then name of the company = name from request
  Then type of the company = type from request
  Then users of the company = users from request
  Examples: 
  | type |
  | ООО  |
  | ОАО  |
  |  ИП  |


  @addAvatar
Scenario: Add avatar
Given email divohi1607@fazmail.net and avatar path ..\..\Resources\user2.png
When add avatar post request to /tasks/rest/addavatar/?email=
Then status code OK

  @deleteAvatar
Scenario: Delete avatar
Given account divohi1607@fazmail.net
When delete avatar post request to /tasks/rest/deleteavatar/?email=
Then account successful created status code OK

  @doRegister
 Scenario: Register new user
 Given name of user QA_Alex
 And email of user
 And user password
 When send post request to do register to /tasks/rest/doregister
 Then account successful created status code OK
 And response have name of user
 And response have email of user
 But response doesn't have a non-encrypted password

  @doRegister @negative
 Scenario: Register exist email
 Given name of user QA_Alex
 And exist email of user divohi1607@fazmail.net
 And user password divohi1607@fazmail.net
 When send post request to do register to /tasks/rest/doregister
 Then account successful created status code OK
 But response type is error
 And response contains <уже есть в базе> and <email>
 

  @doRegister @negative
 Scenario: Register exist user name
 Given exist name of user QA_Alex
 And user email divohi1607@fazmail.net
 And user password divohi1607@fazmail.net
 When send post request to do register to /tasks/rest/doregister
 Then account successful created status code OK
 But response type is error
 And response contains <уже есть в базе> and <Текущее ФИО>
 