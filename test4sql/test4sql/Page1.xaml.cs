﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using PCLStorage;
using SharpCifs.Smb;  // http://sharpcifsstd.dobes.jp/
using System.IO;

namespace test4sql
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]

   
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
            
        }



        


        async void Click_Login(object sender, EventArgs e)
        {
            // To create a new subfolder in the local folder, call the CreateFolderAsync method.
            String folderName = "csharp";
          //  IFolder folder = FileSystem.Current.LocalStorage;
          //  folder = await folder.CreateFolderAsync(folderName, CreationCollisionOption.ReplaceExisting);
          
        }

         void Shared_Folder(object sender, EventArgs e)
        {

            //Get the SmbFile specifying the file name to be created.
            var file = new SmbFile("smb://User:1@192.168.1.5/backpel/New2FileName.txt");

            //Create file.
            file.CreateNewFile();

            //Get writable stream.
            var writeStream = file.GetOutputStream();

            //Write bytes.
            writeStream.Write(Encoding.UTF8.GetBytes("Hello!"));

            //Dispose writable stream.
            writeStream.Dispose();





        }


        void ReadFile(object sender, EventArgs e)
        {
            bool f = ReadFiles();
            //Get the SmbFile specifying the file name to be created.
            var file = new SmbFile("smb://User:1@192.168.1.5/backpel/New2FileName.txt");
            //Get target's SmbFile.
           // var file = new SmbFile("smb://UserName:Password@ServerIP/ShareName/Folder/FileName.txt");

            //Get readable stream.
            var readStream = file.GetInputStream();

            //Create reading buffer.
            var memStream = new MemoryStream();

            //Get bytes.
            ((Stream)readStream).CopyTo(memStream);

            //Dispose readable stream.
            readStream.Dispose();

            Console.WriteLine(Encoding.UTF8.GetString(memStream.ToArray()));

        }


       private bool  ReadFiles()
        {


            //   Get items in shared folder:            https://www.csharpcodi.com/csharp-examples/SharpCifs.Smb.SmbFile.GetUncPath0()/
            //using System;
            //using SharpCifs.Smb;

            //Get SmbFile-Object of a folder.
            var folder = new SmbFile("smb://User:1@192.168.1.5/backpel/");

        //UnixTime
        var epocDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

//List items
        foreach (SmbFile item in folder.ListFiles())
        {
            var lastModDate = epocDate.AddMilliseconds(item.LastModified())
                                .ToLocalTime();
            var name = item.GetName();
            var type = item.IsDirectory() ? "dir" : "file";
            var date = lastModDate.ToString("yyyy-MM-dd HH:mm:ss");
            var msg = $"{name} ({type}) - LastMod: {date}";
            Console.WriteLine(msg);
        }

            return true;
        }












        async void Runsql(object sender, EventArgs e)
        {
            but21.Text = "==" ;
            // Create New file

            // To create a new file in the local folder, call the CreateFileAsync method.
            String filename = "username.txt";
          //  IFolder folder = FileSystem.Current.LocalStorage;
          //  IFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);





        }










    }
}