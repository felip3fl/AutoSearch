using LocalFile.Models;

namespace LocalFile
{
    public class LocalFile
    {
        public List<Record> GetListFileName(string folderAddress)
        {
            var filesWithAddress = Directory.GetFiles(folderAddress);
            var filesName = new List<Record>();

            foreach (var fileAddress in filesWithAddress)
            {
                var localFile = new Record();
                var fileContent = File.ReadLines(fileAddress);

                localFile.Id                = filesName.Count + 1;
                localFile.Name              = Path.GetFileNameWithoutExtension(fileAddress);
                localFile.NameWithExtension = Path.GetFileName(fileAddress);
                localFile.Path              = fileAddress;
                localFile.Extension         = Path.GetExtension(fileAddress);
                localFile.Size              = fileContent.Count();

                filesName.Add(localFile);
            }

            return filesName;
        }


    }
}
