start program 
-> transfer available files
-> show gui
-> enter eventloop watcher

gui handlers - button clear click
-> empty log text box

gui handlers - button hchs click
-> runs disableNAKO script
-> if false set hchs enabled to false and mark button red
-> else
--> set nako button red and hchs button green
--> Show GetPatient form
--> in getPatientform
---> get id, date of birth and sex
---> exec GDT_SendPatient to watchfolder
