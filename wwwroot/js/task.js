// grab the first tab and tab content elements
const activeTab = document.querySelector('ul.nav-tabs li:first-child a');
const activeTabContent = document.querySelector('#myTabContent div.tab-pane:first-child');

// add active class to them 
activeTab?.classList.add('active');
activeTabContent?.classList.add('active');
activeTabContent?.classList.add('show');
