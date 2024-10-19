using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Text.Json;
using Microsoft.VisualBasic.Devices;

namespace MineCleaner
{
    public partial class MineCleaner : Form
    {
        public int mineLimit;
        public int fieldSize;
        public int[,] field;
        int gameState = 0; // 0 - not started, 1 - started, 2 - ended
        
        int mineCount;
        int mineCountX, mineCountY;
        const int mineSpotSize = 22;
        const int firstX = 12;
        const int firstY = 46;

        Image mineImg = Image.FromFile("./assets/mine.png");
        Image flagImg = Image.FromFile("./assets/flag.png");
        Image notABombSpotImg = Image.FromFile("./assets/not_a_bomb_spot.png");
        Image oneImg = Image.FromFile("./assets/1.png");
        Image twoImg = Image.FromFile("./assets/2.png");
        Image threeImg = Image.FromFile("./assets/3.png");
        Image fourImg = Image.FromFile("./assets/4.png");
        Image fiveImg = Image.FromFile("./assets/5.png");
        Image sixImg = Image.FromFile("./assets/6.png");
        Image sevenImg = Image.FromFile("./assets/7.png");
        Image eigthImg = Image.FromFile("./assets/8.png");

        System.Timers.Timer timer = new System.Timers.Timer();
        TimeSpan timerTime = new TimeSpan(0,0,0);

        Label timerLabel = new Label();
        Label mineCountLabel = new Label();

        public MineCleaner()
        {
            InitializeComponent();

            Dictionary<String,int> configValues;
            if (!File.Exists("./config.json")) File.WriteAllText("./config.json", "{\n\"fieldSize\":8\n,\n\"mineLimit\":8\n}");
            String configJson = File.ReadAllText("./config.json");
            try
            {
                configValues = JsonSerializer.Deserialize<Dictionary<String, int>>(configJson);
                fieldSize = configValues["fieldSize"];
                mineLimit = configValues["mineLimit"];
            }
            catch
            {
                File.WriteAllText("./config.json", "{\n\"fieldSize\":8\n,\n\"mineLimit\":8\n}");
                fieldSize = 8;
                mineLimit = 8;
            }

            if (fieldSize < 8) 
            {
                MessageBox.Show($"WARNING!\nInvalid field size!\nField size has been set to 8.");
                fieldSize = 8;
            }

            if (mineLimit >= fieldSize * fieldSize || mineLimit < 1)
            {
                MessageBox.Show($"WARNING!\nInvalid number of mines in field!\nMine number has been set to {fieldSize}.");
                mineLimit = fieldSize;
            }

            field = new int[fieldSize, fieldSize];

            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    Button btn = new Button();

                    btn.Name = $"mineSpot_{i}_{j}";
                    btn.Location = new Point(firstX + (mineSpotSize * j), firstY + (mineSpotSize * i));
                    btn.Size = new Size(mineSpotSize, mineSpotSize);
                    btn.Text = "";
                    btn.Image = null;
                    btn.ImageAlign = ContentAlignment.MiddleCenter;
                    btn.BackColor = Color.FromArgb(44, 44, 44);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(22, 22, 22);
                    btn.TabStop = false;
                    btn.MouseUp += btn_Click;

                    Controls.Add(btn);
                    field[i, j] = 1;
                }
            }

            timerLabel.Name = "timerLabel";
            timerLabel.Location = new Point(firstX, firstY - 18);
            timerLabel.Text = "00:00:00";
            timerLabel.ForeColor = Color.White;
            Controls.Add(timerLabel);

            mineCountLabel.Name = "mineCountLabel";
            mineCountX = firstX + (mineSpotSize * fieldSize) - 50;
            mineCountY = firstY - 18;
            mineCountLabel.Location = new Point(mineCountX, mineCountY);
            mineCountLabel.ForeColor = Color.White;
            Controls.Add(mineCountLabel);
            mineCount = mineLimit;
            mineCountLabel.Text = "";

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimerEvent;
            timer.AutoReset = true;

            this.Size = new Size(firstX * 2 + (mineSpotSize * fieldSize) + 16, firstY * 2 + (mineSpotSize * fieldSize) + 5);
        }

        private void OnTimerEvent(Object source, ElapsedEventArgs e)
        {
            timerTime += TimeSpan.FromSeconds(1);
            String h, m, s;
            if (timerTime.Seconds < 10) s = $"0{timerTime.Seconds}";
            else s = timerTime.Seconds.ToString();
            if (timerTime.Minutes < 10) m = $"0{timerTime.Minutes}";
            else m = timerTime.Minutes.ToString();
            if (timerTime.Hours < 10) h = $"0{timerTime.Hours}";
            else h = timerTime.Hours.ToString();

            timerLabel.Text = $"{h}:{m}:{s}";
        }

        private void regenerateField(int newFieldSize, int newMineLimit)
        {
            timer.Stop();

            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    var button = Controls.OfType<Button>().FirstOrDefault(b => b.Name == $"mineSpot_{i}_{j}");
                    Controls.Remove(button);
                }
            }
            var counter = Controls.OfType<Label>().FirstOrDefault(b => b.Name == $"mineCountLabel");
            Controls.Remove(counter);

            fieldSize = newFieldSize;
            mineLimit = newMineLimit;
            field = new int[fieldSize, fieldSize];

            File.WriteAllText("./config.json", $"{{\n\"fieldSize\":{fieldSize}\n,\n\"mineLimit\":{mineLimit}\n}}");

            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    Button btn = new Button();

                    btn.Name = $"mineSpot_{i}_{j}";
                    btn.Location = new Point(firstX + (mineSpotSize * j), firstY + (mineSpotSize * i));
                    btn.Size = new Size(mineSpotSize, mineSpotSize);
                    btn.Text = "";
                    btn.Image = null;
                    btn.ImageAlign = ContentAlignment.MiddleCenter;
                    btn.BackColor = Color.FromArgb(44, 44, 44);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(22, 22, 22);
                    btn.TabStop = false;
                    btn.MouseUp += btn_Click;

                    Controls.Add(btn);
                    btn.BringToFront();
                    field[i, j] = 1;
                }
            }

            this.Size = new Size(firstX * 2 + (mineSpotSize * fieldSize) + 16, firstY * 2 + (mineSpotSize * fieldSize) + 5);


            mineCountLabel.Name = "mineCountLabel";
            mineCountX = firstX + (mineSpotSize * fieldSize) - 50;
            mineCountY = firstY - 18;
            mineCountLabel.Location = new Point(mineCountX, mineCountY);
            mineCountLabel.ForeColor = Color.White;
            Controls.Add(mineCountLabel);
            mineCount = mineLimit;
            mineCountLabel.Text = "";

            newToolStripMenuItem_Click(null, null);
        }

        private void addMines()
        {
            Random random = new Random();

            int mines = 0;
            while (mines < mineLimit) {
                int i = random.Next(fieldSize);
                int j = random.Next(fieldSize);

                if (field[i, j] == 1)
                {
                    field[i, j] = 2;
                    mines++;
                };
            }

            changeMineCount(mines);
        }

        private int checkMineSpot(int i, int j)
        {
            if (i == -1 || i == fieldSize || j == -1 || j == fieldSize)
            {
                return 0;
            }
            if (field[i, j] == 2 || field[i, j] == 4) return 1;
            if (field[i, j] == 3) return 0;
            return 0;
            
        }

        private void checkNeighbors(int i, int j, bool notCalledByClick)
        {
            if (i == -1 || i == fieldSize || j == -1 || j == fieldSize) return;
            if (field[i, j] == 0 && !notCalledByClick) return;

            int neighborMineCount = 0;
            // Up
            neighborMineCount += checkMineSpot(i - 1, j - 1); // Left
            neighborMineCount += checkMineSpot(i - 1, j); // Middle
            neighborMineCount += checkMineSpot(i - 1, j + 1); // Right

            // Left, Right
            neighborMineCount += checkMineSpot(i, j - 1); // Left
            neighborMineCount += checkMineSpot(i, j + 1); // Right

            // Down
            neighborMineCount += checkMineSpot(i + 1, j - 1); // Left
            neighborMineCount += checkMineSpot(i + 1, j); // Middle
            neighborMineCount += checkMineSpot(i + 1, j + 1); // Right

            field[i, j] = 0;
            var button = Controls.OfType<Button>().FirstOrDefault(b => b.Name == $"mineSpot_{i}_{j}");
            switch (neighborMineCount)
            {
                case 0:
                    setButtonStateToChecked(i, j, true);
                    // Up
                    checkNeighbors(i - 1, j - 1, false); // Left
                    checkNeighbors(i - 1, j, false); // Middle
                    checkNeighbors(i - 1, j + 1, false); // Right

                    // Left, Right
                    checkNeighbors(i, j - 1, false); // Left
                    checkNeighbors(i, j + 1, false); // Right

                    // Down
                    checkNeighbors(i + 1, j - 1, false); // Left
                    checkNeighbors(i + 1, j, false); // Middle
                    checkNeighbors(i + 1, j + 1, false); // Right
                    break;
                case 1:
                    field[i, j] = 0;
                    setButtonStateToChecked(i, j, true);
                    button.Image = oneImg;
                    break;
                case 2:
                    setButtonStateToChecked(i, j, true);
                    button.Image = twoImg;
                    break;
                case 3:
                    setButtonStateToChecked(i, j, true);
                    button.Image = threeImg;
                    break;
                case 4:
                    setButtonStateToChecked(i, j, true);
                    button.Image = fourImg;
                    break;
                case 5:
                    setButtonStateToChecked(i, j, true);
                    button.Image = fiveImg;
                    break;
                case 6:
                    setButtonStateToChecked(i, j, true);
                    button.Image = sixImg;
                    break;
                case 7:
                    setButtonStateToChecked(i, j, true);
                    button.Image = sevenImg;
                    break;
                case 8:
                    setButtonStateToChecked(i, j, true);
                    button.Image = eigthImg;
                    break;
            }
        }

        private void checkForWin()
        {
            int checkedTilesCount = 0;
            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    if (field[i, j] == 0)
                    {
                        checkedTilesCount++;
                    }
                }
            }
            if (checkedTilesCount == fieldSize * fieldSize - mineLimit)
            {
                gameState = 2;
                timer.Stop();
                writeGameRecord(true);

                MessageBox.Show("🏁💣 You Win! 💣🏁");
            }
        }

        private void setButtonStateToChecked(int i, int j, bool setToChecked)
        {
            var button = Controls.OfType<Button>().FirstOrDefault(b => b.Name == $"mineSpot_{i}_{j}");

            if (setToChecked)
            {
                button.Enabled = false;
                button.BackColor = Color.FromArgb(33, 33, 33);
            }
            else
            {
                button.Image = null;
                button.Enabled = true;
                button.BackColor = Color.FromArgb(44, 44, 44);
            }
        }

        private void changeMineCount(int newCount)
        {
            mineCount = newCount;
            mineCountLabel.Text = $"Mines: {mineCount}";
            if ((mineCount > 9 && mineCount < 100) || (mineCount < 0 && mineCount > -10)) mineCountLabel.Location = new Point(mineCountX - 4, mineCountY);
            else if (mineCount > 99 || mineCount < -9) mineCountLabel.Location = new Point(mineCountX - 8, mineCountY);
            else mineCountLabel.Location = new Point(mineCountX, mineCountY);
        }

        private void writeGameRecord(bool isWin)
        {
            DateTime dateAndTime = DateTime.Now;
            char winOrLose;
            if (isWin) winOrLose = 'W';
            else winOrLose = 'L';

            String record = $"{timerTime.Hours}:{timerTime.Minutes}:{timerTime.Seconds}-{fieldSize}-{mineCount}-{mineLimit}-{winOrLose}-{dateAndTime.Day}/{dateAndTime.Month}/{dateAndTime.Year},{dateAndTime.Hour}:{dateAndTime.Minute}:{dateAndTime.Second}";
            if (File.Exists("./records.log")) File.AppendAllText("./records.log", $"{Environment.NewLine}");
            File.AppendAllText("./records.log", record);
        }

        private void btn_Click(object sender, MouseEventArgs e)
        {
            if (gameState == 2) return; 

            if (e.Button == MouseButtons.Left)
            {
                Button btn = (Button)sender;
                String btnName = btn.Name;
                int posI = Int32.Parse((btnName.Substring(btnName.IndexOf('_') + 1, btnName.LastIndexOf('_') - btnName.IndexOf('_') - 1)));
                int posJ = Int32.Parse(btnName.Substring(btnName.LastIndexOf('_') + 1));

                if (field[posI, posJ] == 3 || field[posI, posJ] == 4) return;

                if (gameState == 0)
                {
                    field[posI, posJ] = 0;
                    addMines();
                    field[posI, posJ] = 1;

                    gameState = 1;
                    timer.Start();
                }

                if (field[posI, posJ] == 1)
                {
                    checkNeighbors(posI, posJ, true);
                    checkForWin();
                }
                else if (field[posI, posJ] == 2)
                {
                    gameState = 2;
                    timer.Stop();
                    writeGameRecord(false);

                    for (int i = 0; i < fieldSize; i++)
                    {
                        for (int j = 0; j < fieldSize; j++)
                        {
                            var button = Controls.OfType<Button>().FirstOrDefault(b => b.Name == $"mineSpot_{i}_{j}");
                            if (field[i, j] == 2) button.Image = mineImg;
                            else if (field[i, j] == 3) button.Image = notABombSpotImg;
                        }
                    }
                }
            }

            else if (e.Button == MouseButtons.Right)
            {
                if (gameState == 0 || gameState == 2) return;

                Button btn = (Button)sender;
                String btnName = btn.Name;
                int posI = Int32.Parse((btnName.Substring(btnName.IndexOf('_') + 1, btnName.LastIndexOf('_') - btnName.IndexOf('_') - 1)));
                int posJ = Int32.Parse(btnName.Substring(btnName.LastIndexOf('_') + 1));

                // 0 --> Checked
                // 1 --> Clear
                // 2 --> Mine
                // 3 --> Flagged (Clear)
                // 4 --> Flagged (Mine)

                switch (field[posI, posJ])
                {
                    case 1:
                        field[posI, posJ] = 3;
                        btn.Image = flagImg;
                        changeMineCount(mineCount - 1);
                        break;
                    case 2:
                        field[posI, posJ] = 4;
                        btn.Image = flagImg;
                        changeMineCount(mineCount - 1);
                        break;
                    case 3:
                        field[posI, posJ] = 1;
                        btn.Image = null;
                        changeMineCount(mineCount + 1);
                        break;
                    case 4:
                        field[posI, posJ] = 2;
                        btn.Image = null;
                        changeMineCount(mineCount + 1);
                        break;
                }
            }
        }

        private void showFieldArrayDEBUG()
        {
            String debug = "";
            for (int i = 0; i < fieldSize; i++)
            {
                debug += "\n";
                for (int j = 0; j < fieldSize; j++)
                {
                    debug += $"{field[i, j]} ";
                }
            }
            debug += "\r\n\r\n0 --> Checked\r\n1 --> Clear\r\n2 --> Mine\r\n3 --> Flagged (Clear)\r\n4 --> Flagged (Mine)";
            MessageBox.Show(debug);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameState = 0;
            timerLabel.Text = "00:00:00";
            timerTime = TimeSpan.Zero;
            mineCount = 0;
            mineCountLabel.Text = "";

            for (int i = 0; i < fieldSize; i++)
            {
                for (int j = 0; j < fieldSize; j++)
                {
                    field[i, j] = 1;
                    setButtonStateToChecked(i, j, false);
                }
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsMenu form2 = new SettingsMenu();
            form2.ShowDialog();

            regenerateField(form2.fieldSize, form2.mineLimit);
        }

        private void recordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("./records.log"))
            {
                MessageBox.Show("No records found!");
                return;
            }
            RecordsForm form3 = new RecordsForm();
            form3.ShowDialog();
        }

        private void debugToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            showFieldArrayDEBUG();
        }
    }
}
