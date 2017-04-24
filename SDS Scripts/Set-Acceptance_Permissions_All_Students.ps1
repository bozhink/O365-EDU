<#
Script Name:
Set-Acceptance_Permissions_All_Students.ps1

Synopsis:
This script is designed to add the All_Students DDL to the acceptance permissions on each of the user's mailbox, within the scope of this script. After running this script, only members of the All_Students DDL will be able to email that particular user. In order to run their script successfully, you must run the New-DDL_for_All_Students.ps1 to create and populate the DDL to be used here.

Syntax Examples and Options:
.\Set-Acceptance_Permissions_All_Students.ps1

Written By: 
Bill Sluss

Change Log:
Version 1.0, 12/14/2016 - First Draft

#>


#Import the CSV we exported
$Users = Get-MsolUser -All | ? {$_.Department -eq "Student"}

#Set Global Variables
$DDL = Get-DynamicDistributionGroup All_Students
$DDA = $DDL.PrimarySmtpAddress
$DDN = $DDL.DisplayName

#Start Foearch loop against the initial export
Foreach ($User in $Users) {

    #Set some variables
    $DN = $User.DisplayName
    $ObjID = $User.ObjectID

    #Write Progress
    Write-Host -ForegroundColor Green "Adding $DDN to $DN acceptance list"

    #Set the permissions
    Set-Mailbox $ObjID -AcceptMessagesOnlyFromDLMembers @{add = $DDA}
}
