using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerrariaDepotDownloader
{
    public class ControlWriter : TextWriter
    {
        private readonly Control textbox;
        public ControlWriter(Control textbox)
        {
            this.textbox = textbox;
        }

        public async override void Write(char value)
        {
            // Append each character to the file.
            try
            {
                // Open the file in append mode using StreamWriter.
                using (StreamWriter writer = new StreamWriter(Application.StartupPath + @"\Log.txt", true))
                {
                    // Write the character to the file.
                    await writer.WriteAsync(value);
                }
            }
            catch (Exception)
            { }

            textbox.Text += value;
        }

        public override void Write(string value)
        {
            textbox.Text += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }

    public class MultiTextWriter : TextWriter
    {
        private readonly IEnumerable<TextWriter> writers;
        public MultiTextWriter(IEnumerable<TextWriter> writers)
        {
            this.writers = writers.ToList();
        }
        public MultiTextWriter(params TextWriter[] writers)
        {
            this.writers = writers;
        }

        public override void Write(char value)
        {
            try
            {
                foreach (var writer in writers)
                    writer.Write(value);
            }
            catch (Exception)
            {
            }
        }

        public override void Write(string value)
        {
            try
            {
                foreach (var writer in writers)
                    writer.Write(value);
            }
            catch (Exception)
            {
            }
        }

        public override void Flush()
        {
            try
            {
                foreach (var writer in writers)
                    writer.Flush();
            }
            catch (Exception)
            {
            }
        }

        public override void Close()
        {
            try
            {
                foreach (var writer in writers)
                    writer.Close();
            }
            catch (Exception)
            {
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
