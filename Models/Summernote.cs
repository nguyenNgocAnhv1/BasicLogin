namespace App.Models
{
     public class Summernote
     {
          public Summernote(string idEditor, bool loadLib, int height=120)
          {
               IdEditor = idEditor;
               this.loadLib = loadLib;
               this.height = height;
          }

          public string IdEditor { get; set; }
          public bool loadLib { get; set; }
          public int height { get; set; }
         


     }
}