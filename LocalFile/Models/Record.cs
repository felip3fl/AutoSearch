namespace LocalFile.Models
{
    public class Record
    {
        public Record()
        {

        }

        public Record(string name, string nameWithExtension, string path, string extension, int size)
        {
            Name = name;
            NameWithExtension = nameWithExtension;
            Path = path;
            Extension = extension;
            Size = size;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameWithExtension { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public int Size { get; set; }
    }
}
