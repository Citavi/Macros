using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Collections;

// Implementation of macro editor is preliminary and experimental.
// The Citavi object model is subject to change in future version.

public static class CitaviMacro
{
	//SetAllFilesPreviewBehaviourToSkipEntryPage 1.1
	public static void Main()
	{
		if (Program.ProjectShells.Count == 0) return;		//no project open	
		if (IsBackupAvailable() == false) return;			//user wants to backup his/her project first
		
		int counter = 0;
		
		try 
		{
			List<Reference> references = Program.ActiveProjectShell.PrimaryMainForm.GetFilteredReferences();
			SwissAcademic.Citavi.Project activeProject = Program.ActiveProjectShell.Project;
	
			List<Location> filesToShowImmediately = new List<Location>(); 
			
			foreach (Reference reference in references)
			{	
				foreach (Location location in reference.Locations)
				{
					//first we collect all files that are not set to be shown immediately yet
					if (location.LocationType == LocationType.ElectronicAddress)
					{
						switch(location.AddressUri.AddressInfo)
						{
							case ElectronicAddressInfo.RemoteUri:
							case ElectronicAddressInfo.AbsoluteFileUri:
							case ElectronicAddressInfo.RelativeFileUri:
							case ElectronicAddressInfo.CitaviFiles:
							{
								//MessageBox.Show(string.Format("Location:{0}\n\rPreview Behaviour:{1}\n\rLocal Path:{2}", location.ToString(), location.PreviewBehaviour.ToString(), location.AddressUri.AbsoluteUri.LocalPath.ToString()));
								if(location.PreviewBehaviour != PreviewBehaviour.SkipEntryPage) filesToShowImmediately.Add(location);
								//if(location.AddressUri.AbsoluteUri.LocalPath == DriveType.Network) filesToShowImmediately.Add(location);
							}
							break;
						}
					}
				}
			}
		
			
			//now we change the PreviewBehaviour on all the collected files
			foreach(Location location in filesToShowImmediately)
			{
				if (location.PreviewBehaviour != PreviewBehaviour.SkipEntryPage) 
				{
					location.PreviewBehaviour = PreviewBehaviour.SkipEntryPage;
					counter++;
				}
			}
			
			activeProject.Save(TrackPriority.Immediate);
			
		} //end try
		
		finally 
		{
			MessageBox.Show(string.Format("Macro has finished execution.\r\n{0} changes were made.", counter.ToString()), "Citavi", MessageBoxButtons.OK, MessageBoxIcon.Information);
		} //end finally
		
	} //end main()
	
	private static bool IsBackupAvailable() 
	{
		string warning = String.Concat("Important: This macro will make irreversible changes to your project.",
			"\r\n\r\n", "Make sure you have a current backup of your project before you run this macro.",
			"\r\n", "If you aren't sure, click Cancel and then, in the main Citavi window, on the File menu, click Create backup.",
			"\r\n\r\n", "Do you want to continue?"
		);
				
		return (MessageBox.Show(warning, "Citavi", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.OK);

	} //end IsBackupAvailable()	
}