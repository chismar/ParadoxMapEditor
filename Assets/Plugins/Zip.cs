using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Ionic.Zip;
using System.Text;
using System.IO;

public class ZipUtil
{


	public static void Unzip (string zipFilePath, string location)
	{
		Directory.CreateDirectory (location);
		
		using (ZipFile zip = ZipFile.Read (zipFilePath)) {
			
			zip.ExtractAll (location, ExtractExistingFileAction.OverwriteSilently);
		}
	}

	public static void Zip (string zipFileName, params string[] files)
	{
		string path = Path.GetDirectoryName(zipFileName);
		Directory.CreateDirectory (path);
		
		using (ZipFile zip = new ZipFile()) {
			foreach (string file in files) {
				zip.AddFile(file, "");
			}
			zip.Save (zipFileName);
		}
	}

	public static void ZipDirectory (string zipFileName, string dir)
	{
		string path = Path.GetDirectoryName(zipFileName);
		Directory.CreateDirectory (path);

		using (ZipFile zip = new ZipFile()) {
			zip.AddDirectory (dir);
			zip.Save (zipFileName);
		}
	}
}
