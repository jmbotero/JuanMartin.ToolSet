using JuanMartin.LabeMaker;
using JuanMartin.RestApiClient;
using System.Collections.Generic;

namespace JuanMartin.ToolSet.LabeMaker
{
    public class DVDLabelMaker
    {
        public void Create(string title, string year = "")
        {
            var doc = new OdtFile(@"x:\JuanMartin\ToolSet\LabelMaker\dvd-labels.odt", use_template: true);
            var api = new ImdbApi();
            var updates = new Dictionary<string, string>();

            var m = api.GetMovie(title, year);

            updates.Add("JuanMartin.title", m.title);
            updates.Add("JuanMartin.year", m.year);

            var d = m.duration;
            int hours = (d - d % 60) / 60;
            int minutes = d - hours * 60;
            updates.Add("JuanMartin.duration", string.Format("{0}h{1}m", hours, minutes));
            updates.Add("JuanMartin.directors", string.Join(",", m.directors.ToArray()));
            updates.Add("JuanMartin.genres", string.Join(",", m.genres.ToArray()));
            updates.Add("JuanMartin.plot", m.plot);

            doc.Update(updates);
        }
    }
}