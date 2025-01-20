using LocalFile.Models;

namespace LocalFile
{
    public class LocalFile
    {
        public List<Record> GetListFileName(string folderAddress)
        {
            var filesWithAddress = Directory.GetFiles(folderAddress);
            var filesName = new List<Record>();

            foreach (var item in filesWithAddress)
            {
                var localFile = new Record();

                localFile.Id                = filesName.Count + 1;
                localFile.Name              = Path.GetFileNameWithoutExtension(item);
                localFile.NameWithExtension = Path.GetFileName(item);
                localFile.Path              = item;
                localFile.Extension         = Path.GetExtension(item);

                filesName.Add(localFile);
            }

            return filesName;
        }


    }
}
