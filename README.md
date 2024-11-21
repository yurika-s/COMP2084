# Housework Manager
## COMP2084 Server-Side Scripting-ASP.NET - Yurika Sugita # 200558167

### Link

https://housework-manager.azurewebsites.net/

### Purpose of the application

This application is for managing housework (tasks) and sharing that status with members in a group.
Sharing housework is a big issue in shared houses and can often cause trouble. The app encourages group members to live together smoothly by showing detailed tasks and dates and assigning them equally.

### Main target

Family members and persons who live in shared houses.

### Additional advanced .NET MVC features

- Showing different "Home" pages between before and after login(Before: Welcome page which describes the application, after: tasks list)
- Collecting data with filter conditions
  - Home (after login): task data only whose assignee is the current login user and Done is false (unfinished tasks) ordered by deadline date
  - Groups (index): groups only the login user created or belongs to
  - Groups (details): group members only who belong to the group on the page
  - Tasks (index): show tasks by each group the login user belongs to
  - Tasks (create/edit): limiting select options that only groups the login user belongs to and users who belong to the filtered groups
- Setting default value when new items are created
  - GroupMembers: GroupID -> the group the user opened on the details page before clicking "Add member" button
- Redirecting to different actions in another controller after process in an action
  - GroupMembers (create/edit/delete): it redirects to Groups/Details page the user opened before the action
- On Tasks create/edit/delete pages, return to different pages when clicking on "back to list" depends on where user came from: Home or Tasks Index page

## Lab 3: GitHub Login
Implemented register and login with GitHub account

## Assignment 3
- Created an account: prof@gc.ca / Test123$
- Make all Methods and View links where users can add, edit, or delete data PRIVATE, so only authenticated users can access them  
 -> It has already been implemented in Assignment 2
- On your Index views, anonymous users can view the list of data but cannot see the Create, Edit, or Delete links

- Enabled Social Authentication with Google
