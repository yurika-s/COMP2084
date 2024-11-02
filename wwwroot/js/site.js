// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// get url path name
const urlPaths = location.pathname.split('/');
const controller = urlPaths[1]; 

// grab the nav list elements
const navHome = document.querySelector('ul.navbar-nav li.nav-item:nth-child(1) a');
const navGroup = document.querySelector('ul.navbar-nav li.nav-item:nth-child(2) a');
const navTask = document.querySelector('ul.navbar-nav li.nav-item:nth-child(3) a');

// remove active class from all nav (reset) 
navHome?.classList.remove('active');
navGroup?.classList.remove('active');
navTask?.classList.remove('active');

// add active class to nav based on current page
switch (controller) {
    case '':
        navHome?.classList.add('active');
        break;
    case 'Groups':
    case 'GroupMembers':
        navGroup?.classList.add('active');
        break;
    case 'Tasks':
        navTask?.classList.add('active');
        break;
}