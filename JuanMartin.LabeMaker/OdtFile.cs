using JuanMartin.Kernel.Utilities;
using System.Collections.Generic;
using System.IO;

namespace JuanMartin.LabeMaker
{
    public class OdtFile
    {

        private const string CONTENT_FILENAME = "content.xml";
        private bool _use_template;
        private string _original_file_name;
        private string _template_file_name;

        public string Name { get; set; }
        public OdtFile(string name, bool use_template = false)
        {
            _use_template = use_template;

            if (use_template)
            {
                Name = Path.GetTempFileName();
                _original_file_name = name;
                _template_file_name = $"{Path.GetDirectoryName(name)}\\{Path.GetFileNameWithoutExtension(name)}-template.{Path.GetExtension(name)}";
            }
            else
            {
                Name = name;
                _original_file_name = string.Empty;
                _template_file_name = string.Empty;
            }
        }

        public void Update(Dictionary<string, string> changes)
        {
            if (_use_template)
                PreFileTemplateProcesing();

            var text = UtilityFile.GetTextContentOfFileInZip(Name, CONTENT_FILENAME);

            foreach (var update in changes)
            {
                text = text.Replace(update.Key, update.Value);
            }

            UtilityFile.UpddateTextContentOfFileInZip(Name, CONTENT_FILENAME, text);

            if (_use_template)
                PostFileTemplateProcesing();
        }

        private void PreFileTemplateProcesing()
        {
            if (File.Exists(Name))
                File.Delete(Name);
            File.Copy(_template_file_name, Name);
        }

        private void PostFileTemplateProcesing()
        {
            if (File.Exists(_original_file_name))
                File.Delete(_original_file_name);
            File.Copy(Name, _original_file_name);
        }
    }
}
