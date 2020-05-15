using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Fighting_2
{
    class GameForm : Form
    {
        public GameForm(GameModel game)
        {
            Size = new Size(900, 550);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            SetFighterAbilities(game);
            SetFightersStats(game, game.Fighters[GameModel.Side.AI], 450);
            SetFightersStats(game, game.Fighters[GameModel.Side.Player], 200);
            InitShowTargets(game, 300);
            InitGameOver(game);
        }
        private void SetFighterAbilities(GameModel game)
        {
            Action<Fighter> d = (fighter) =>
            {
                Controls.RemoveByKey("targetTable");
                var table = new TableLayoutPanel()
                {
                    Location = new Point(50, 350),
                    RowCount = 1,
                    ColumnCount = fighter.Abilities.Count,
                    Width = fighter.Abilities.Count * 125,
                    Name = "table",
                };
                for (var i = 0; i < table.ColumnCount; i++)
                {
                    var iColumn = i;
                    var button = new Button();
                    button.Text = fighter.Abilities[i].Description;
                    button.Width = 120;
                    button.Dock = DockStyle.Fill;
                    button.Click += (sender, args) => game.ChooseTargetForAbility(fighter.Abilities[iColumn]); // Здесь будет вызван выбор цели
                    table.Controls.Add(button);
                    button.BackColor = Color.FromArgb(155, 155, 255);
                    button.Font = new Font("TimesNewRoman", 14);
                }
                table.Width = 400;
                Controls.Add(table);
            };
            d(game.Fighters[game.side][game.indexOfActiveFighter]);
            game.FighterChanged += d;
        }
        private void SetFightersStats(GameModel game, List<Fighter> fighters, int x)
        {
            Action c = () =>
            {
                Controls.RemoveByKey(x.ToString() + "statsTable");
                var table = new TableLayoutPanel();
                table.RowCount = fighters.Count * 3;
                table.Width = 30;
                table.Height = fighters.Count * 100;
                table.ColumnCount = 1;
                table.Location = new Point(x, 0);
                table.Name = x.ToString() + "statsTable";
                for (var i = 0; i < fighters.Count; i++)
                {
                    var healthLabel = new Label();
                    healthLabel.ForeColor = Color.Red;
                    healthLabel.Width = 25;
                    fighters[i].HealthChanged += f => healthLabel.Text = f.Health.ToString();
                    healthLabel.Text = fighters[i].Health.ToString();
                    table.Controls.Add(healthLabel);
                    var manaLabel = new Label();
                    manaLabel.ForeColor = Color.Blue;
                    manaLabel.Width = 25;
                    fighters[i].ManaChanged += f => manaLabel.Text = f.Mana.ToString();
                    manaLabel.Text = fighters[i].Mana.ToString();
                    table.Controls.Add(manaLabel);
                    table.Controls.Add(new Label() { Height = 70 });
                }                
                Controls.Add(table);
                SetFightersPictures(game, fighters, x);
                table.BringToFront();
                table.BackColor = Color.FromArgb(1, 0, 255, 100);
            };
            c();
            game.Death += c;            
        }
        private void SetFightersPictures(GameModel game, List<Fighter> fighters, int x)
        {
            Action c = () => {
                Controls.RemoveByKey(x + "picturesTable");
                var table = new TableLayoutPanel();
                table.Name = x + "picturesTable";
                table.RowCount = fighters.Count * 2;
                table.Height = 110 * fighters.Count;
                table.Location = new Point(x - 100, 0);
                table.ColumnCount = 1;
                for (var i = 0; i < fighters.Count; i++)
                {
                    var picture = InitPicture(fighters[i]);
                    table.Controls.Add(picture);
                    table.Controls.Add(new Label() { Text = fighters[i].Name});
                }
                Controls.Add(table);                
            };
            c();           
        }
        private PictureBox InitPicture(Fighter fighter)
        {
            var picture = new PictureBox { Image = Image.FromFile(string.Format("pictures/{0}.bmp", fighter.Name.ToLower())) };
            picture.Height = 80;
            picture.MouseHover += (sender, args) =>
            {
                Controls.RemoveByKey("descriptionTable");
                Controls.RemoveByKey("flavorLabel");
                Controls.Add(InitDescriptionTableForFighter(fighter));
                Controls.Add(new Label { Text = fighter.Description, Name = "flavorLabel", Location = new Point(550, 400),
                                            Height = 80, Width = 320, BackColor = Color.FromArgb(200, 255, 200)});
            };
            picture.MouseLeave += (sender, args) => { Controls.RemoveByKey("descriptionTable"); Controls.RemoveByKey("flavorLabel");  };
            return picture;
        }
        private TableLayoutPanel InitDescriptionTableForFighter(Fighter fighter)
        {
            var descriptionTable = new TableLayoutPanel()
            {
                ColumnCount = 2,
                RowCount = fighter.Abilities.Count + 1
            };
            descriptionTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.OutsetDouble;
            foreach (var ability in fighter.Abilities)
            {
                var abilityPicture = new PictureBox { Image = Image.FromFile(string.Format("abilityImages/{0}.bmp", ability.Name)) };
                abilityPicture.Height = 100;
                descriptionTable.Controls.Add(abilityPicture);
                descriptionTable.Controls.Add(new Label { Text = ability.Description, Height = 100, Width = 200 });
            }
            descriptionTable.Controls.Add(new Label { Text = "Effects:"});
            var labelText = new StringBuilder();
            foreach (var e in fighter.Effects)
                labelText.Append(e.Key.ToString() + " = " + e.Value.Value + " for " + e.Value.Duration + " turns; ");
            descriptionTable.Controls.Add(new Label { Text = labelText.ToString(), Height = 60 });
            descriptionTable.Name = "descriptionTable";
            descriptionTable.Location = new Point(550, 0);
            descriptionTable.Height = descriptionTable.RowCount * 100;
            descriptionTable.Width = 325;
            return descriptionTable;
        }
        private void InitShowTargets(GameModel game, int x)
        {
            game.PlayerChooseTarget += (ability, targetsList) =>
            {
                Controls.RemoveByKey("table");
                var table = new TableLayoutPanel();
                table.Location = new Point(x + 250, 50);
                table.RowCount = targetsList.Count;
                table.ColumnCount = 1;
                table.Name = "targetTable";
                for (var i = 0; i < targetsList.Count; i++)
                {
                    var button = new Button();
                    button.Text = targetsList[i].Name;
                    button.BackColor = Color.OrangeRed;
                    int ii = i;
                    button.Location = new Point(x + 50, 50 + ii * 70);
                    button.Click += (sender, args) => game.ChoosedTarget(game.Fighters[game.side][game.indexOfActiveFighter],
                                                                                    ability,
                                                                                    targetsList[ii]);
                    table.Controls.Add(button);
                }
                Controls.Add(table);
            };
        }
        private void InitGameOver(GameModel game)
        {
            game.GameOver += (side) => 
            {
                var label = new Label
                {
                    Text = side.ToString() + " is victorious!",
                    Location = new Point(0, 0),
                    Size = new Size(900, 550),
                    Font = new Font("Arial", 72),
                };
                Controls.Add(label);
                System.Threading.Thread.Sleep(20);
            };
        }
    }
}
