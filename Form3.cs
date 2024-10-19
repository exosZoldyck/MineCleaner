using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineCleaner
{
    public partial class RecordsForm : Form
    {
        bool errored = false;

        public RecordsForm()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            String rawRecordsFileText = File.ReadAllText("./records.log");
            String[] records = rawRecordsFileText.Split(Environment.NewLine);
            foreach (String record in records)
            {
                String recordData = record;

                String gameEndTime = recordData.Substring(0, recordData.IndexOf("-"));
                String hours, minutes, seconds;
                hours = gameEndTime.Substring(0, gameEndTime.IndexOf(":"));
                gameEndTime = gameEndTime.Substring(gameEndTime.IndexOf(":") + 1);
                minutes = gameEndTime.Substring(0, gameEndTime.IndexOf(":"));
                gameEndTime = gameEndTime.Substring(gameEndTime.IndexOf(":") + 1);
                seconds = gameEndTime;
                if (Int32.Parse(hours) < 10) hours = $"0{hours}";
                if (Int32.Parse(minutes) < 10) minutes = $"0{minutes}";
                if (Int32.Parse(seconds) < 10) seconds = $"0{seconds}";
                gameEndTime = $"{hours}:{minutes}:{seconds}";
                recordData = recordData.Substring(recordData.IndexOf("-") + 1);

                String fieldSize = recordData.Substring(0, recordData.IndexOf("-"));
                recordData = recordData.Substring(recordData.IndexOf("-") + 1);

                String mineCount;
                if (recordData.Substring(0, 1) == "-")
                {
                    recordData = recordData.Substring(recordData.IndexOf("-") + 1);
                    mineCount = "-" + recordData.Substring(0, recordData.IndexOf("-"));
                }
                else mineCount = recordData.Substring(0, recordData.IndexOf("-"));
                recordData = recordData.Substring(recordData.IndexOf("-") + 1);

                String mineLimit = recordData.Substring(0, recordData.IndexOf("-"));
                recordData = recordData.Substring(recordData.IndexOf("-") + 1);

                String gameResultChar = recordData.Substring(0, recordData.IndexOf("-"));
                recordData = recordData.Substring(recordData.IndexOf("-") + 1);
                String gameResult = "null";
                if (gameResultChar == "W") gameResult = "WIN";
                else if (gameResultChar == "L") gameResult = "LOSS";

                String date = recordData.Substring(0, recordData.IndexOf(","));
                recordData = recordData.Substring(recordData.IndexOf(",") + 1);
                String time = recordData;

                String recordFinal = "null";
                try
                {
                    recordFinal = $"{gameResult}: {gameEndTime} - {fieldSize}x{fieldSize} - {Int32.Parse(mineLimit) - Int32.Parse(mineCount)}/{mineLimit}💣 - {time}; {date}";
                }
                catch
                {
                    if (!errored) MessageBox.Show("Warning: Invalid values in records file!");
                    errored = true;
                }

                textBox1.Text += $"{recordFinal}\r\n";
            }
        }
    }
}
