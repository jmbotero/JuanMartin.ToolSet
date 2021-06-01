using JuanMartin.Kernel.Utilities;
using System.Collections.Generic;
using System.IO;

namespace JuanMartin.LabeMaker
{
    public class OdtFile
    {

        private const string CONTENT_FILENAME = "content.xml";
        private readonly bool _useTemplate;
        private readonly string _originalFileName;
        private readonly string _templateFileName;

        public string Name { get; set; }
        public OdtFile(string name, bool usTemplate = false)
        {
            _useTemplate = usTemplate;

            if (usTemplate)
            {
                Name = Path.GetTempFileName();
                _originalFileName = name;
                _templateFileName = $"{Path.GetDirectoryName(name)}\\{Path.GetFileNameWithoutExtension(name)}-template.{Path.GetExtension(name)}";
            }
            else
            {
                Name = name;
                _originalFileName = string.Empty;
                _templateFileName = string.Empty;
            }
        }

        public void Update(Dictionary<string, string> changes)
        {
            if (_useTemplate)
                PreFileTemplateProcesing();

            var text = UtilityFile.GetTextContentOfFileInZip(Name, CONTENT_FILENAME);

            foreach (var update in changes)
            {
                text = text.Replace(update.Key, update.Value);
            }

            UtilityFile.UpddateTextContentOfFileInZip(Name, CONTENT_FILENAME, text);

            if (_useTemplate)
                PostFileTemplateProcesing();
        }

        private void PreFileTemplateProcesing()
        {
            if (File.Exists(Name))
                File.Delete(Name);
            File.Copy(_templateFileName, Name);
        }

        private void PostFileTemplateProcesing()
        {
            if (File.Exists(_originalFileName))
                File.Delete(_originalFileName);
            File.Copy(Name, _originalFileName);
        }
    }
}
