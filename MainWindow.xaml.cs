using System.Data;
using System.Windows;
using System.Windows.Controls;
using Npgsql;

namespace v2_database
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private string connString = "Host=localhost;Port=5433;Username=postgres;Password=password;Database=inventory";

        public class Car
        {
            public int ColCarId { get; set; }
            public string ColMarka { get; set; }
            public string ColColor { get; set; }
            public string ColRegNom { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            UpdatecarsDataGrid();
            UpdateOwnersView();
        }

        // Owners
        private void ownersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный объект
            var selectedPerson = ownersDataGrid.SelectedItem as Owner;

            if (selectedPerson != null)
            {
                // Отображаем данные в полях формы
                CarId.Text = selectedPerson.ColCarId.ToString();
                Name.Text = selectedPerson.ColName;
            }
        }
        private void UpdateOwnersView()
        {
            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                string query = "SELECT * FROM owners";
                var cmd = new NpgsqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                var owners = new List<Owner>();
                while (reader.Read())
                {
                    owners.Add(new Owner
                    {
                        ColOwnerId = reader.GetInt32(0),
                        ColCarId = reader.GetInt32(1),
                        ColName = reader.GetString(2)
                    });
                }
                ownersDataGrid.ItemsSource = owners;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении: " + ex.Message);
            }
        }

        private void OwnerButtonAdd(object sender, RoutedEventArgs e)
        {

            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                string query = "INSERT INTO owners (car_id, name) VALUES (@carId, @name)";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("carId", int.Parse(CarId.Text));
                cmd.Parameters.AddWithValue("name", Name.Text);
                cmd.ExecuteNonQuery();
                CarId.Text = "";
                Name.Text = "";
                MessageBox.Show("Запись добавлена!");
                UpdateOwnersView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении: " + ex.Message);
            }
        }

        private void OwnerButtonSave(object sender, RoutedEventArgs e)
        {
            if (ownersDataGrid.SelectedItem is Owner selectedOwner)
            {
                using var conn = new NpgsqlConnection(connString);
                try
                {
                    conn.Open();
                    string query = "UPDATE owners SET car_id = @car_id, name = @name WHERE owner_id = @owner_id";
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("owner_id", selectedOwner.ColOwnerId);
                    cmd.Parameters.AddWithValue("car_id", int.Parse(CarId.Text));
                    cmd.Parameters.AddWithValue("name", Name.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись сохранена!");
                    UpdateOwnersView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                }
            }
        }
        private void OwnerButtonDelete(object sender, RoutedEventArgs e)
        {
            if (ownersDataGrid.SelectedItem is Owner selectedOwner)
            {
                using var conn = new NpgsqlConnection(connString);
                try
                {
                    conn.Open();
                    string query = "DELETE FROM owners WHERE owner_id = @owner_id";
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("owner_id", selectedOwner.ColOwnerId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись удалена!");
                    UpdateOwnersView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }

        private void OwnerButtonSearch(object sender, RoutedEventArgs e)
        {

            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                string query = "SELECT * FROM owners WHERE car_id = @carId OR name ILIKE @name";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("carId", int.Parse(CarId.Text));
                cmd.Parameters.AddWithValue("name", Name.Text);
                var reader = cmd.ExecuteReader();
                var owners = new List<Owner>();
                while (reader.Read())
                {
                    owners.Add(new Owner
                    {
                        ColOwnerId = reader.GetInt32(0),
                        ColCarId = reader.GetInt32(1),
                        ColName = reader.GetString(2)
                    });
                }
                CarId.Text = "";
                Name.Text = "";
                ownersDataGrid.ItemsSource = owners;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске: " + ex.Message);
            }
        }

        private void OwnerButtonUpdate(object sender, RoutedEventArgs e)
        {
            UpdateOwnersView();
            CarId.Text = "";
            Name.Text = "";
        }

        //end Owners

        //Cars
        private void carsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный объект
            var selectedPerson = carsDataGrid.SelectedItem as Car;

            if (selectedPerson != null)
            {
                // Отображаем данные в полях формы
                Marka.Text = selectedPerson.ColMarka;
                Color.Text = selectedPerson.ColColor;
                RegNom.Text = selectedPerson.ColRegNom;
            }
        }
  
        private void UpdatecarsDataGrid()
        {
            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                string query = "SELECT * FROM cars";
                var cmd = new NpgsqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                var cars = new List<Car>();
                while (reader.Read())
                {
                    cars.Add(new Car
                    {
                        ColCarId = reader.GetInt32(0),
                        ColMarka = reader.GetString(1),
                        ColColor = reader.GetString(2),
                        ColRegNom = reader.GetString(3)
                    });
                }
                carsDataGrid.ItemsSource = cars;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении: " + ex.Message);
            }
        }

        private void ButtonSave(object sender, RoutedEventArgs e)
        {
            if (carsDataGrid.SelectedItem is Car selectedCar)
            {
                using var conn = new NpgsqlConnection(connString);
                try
                {
                    conn.Open();
                    string query = "UPDATE cars SET marka = @marka, color = @color, reg_nom = @reg_nom WHERE car_id = @car_id";
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("car_id", selectedCar.ColCarId);
                    cmd.Parameters.AddWithValue("marka", Marka.Text);
                    cmd.Parameters.AddWithValue("color", Color.Text);
                    cmd.Parameters.AddWithValue("reg_nom", RegNom.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись сохранена!");
                    UpdatecarsDataGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                }
            }
        }

        private void ButtonDelete(object sender, RoutedEventArgs e)
        {
            if (carsDataGrid.SelectedItem is Car selectedCar)
            {
                using var conn = new NpgsqlConnection(connString);
                try
                {
                    conn.Open();
                    string query = "DELETE FROM cars WHERE car_id = @car_id";
                    var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("car_id", selectedCar.ColCarId);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись удалена!");
                    UpdatecarsDataGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }

        private void ButtonAdd(object sender, RoutedEventArgs e)
        {
            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                string query = "INSERT INTO cars (marka, color, reg_nom) VALUES (@marka, @color, @reg_nom)";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("marka", Marka.Text);
                cmd.Parameters.AddWithValue("color", Color.Text);
                cmd.Parameters.AddWithValue("reg_nom", RegNom.Text);
                cmd.ExecuteNonQuery();
                Marka.Text = "";
                Color.Text = "";
                RegNom.Text = "";
                MessageBox.Show("Запись добавлена!");
                UpdatecarsDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении: " + ex.Message);
            }
        }

        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                string query = "SELECT * FROM cars WHERE marka ILIKE @marka OR color ILIKE @color OR reg_nom ILIKE @reg_nom";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("marka", Marka.Text);
                cmd.Parameters.AddWithValue("color", Color.Text);
                cmd.Parameters.AddWithValue("reg_nom", RegNom.Text);
                var reader = cmd.ExecuteReader();
                var cars = new List<Car>();
                while (reader.Read())
                {
                    cars.Add(new Car
                    {
                        ColCarId = reader.GetInt32(0),
                        ColMarka = reader.GetString(1),
                        ColColor = reader.GetString(2),
                        ColRegNom = reader.GetString(3)
                    });
                }
                Marka.Text = "";
                Color.Text = "";
                RegNom.Text = "";
                carsDataGrid.ItemsSource = cars;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске: " + ex.Message);
            }
        }

        private void ButtonExit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonUpdate(object sender, RoutedEventArgs e)
        {
            UpdatecarsDataGrid();
            Marka.Text = "";
            Color.Text = "";
            RegNom.Text = "";
        }

        //end Cars

        //info
        private void InfoButtonShow(object sender, RoutedEventArgs e)
        {
            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                string query = "SELECT o.name AS OwnerName, c.marka AS Marka, c.color AS Color, c.reg_nom AS RegNumber FROM owners as o INNER JOIN cars as c ON o.car_id = c.car_id";
                var cmd = new NpgsqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                var info = new List<Info>();
                while (reader.Read())
                {
                    info.Add(new Info
                    {
                        user_name = reader.GetString(0),
                        marka = reader.GetString(1),
                        color = reader.GetString(2),
                        regnom = reader.GetString(3)
                    });
                }
                infoDataGrid.ItemsSource = info;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении: " + ex.Message);
            }
        }

        private void InfoButtonSearch(object sender, RoutedEventArgs e)
        {
            using var conn = new NpgsqlConnection(connString);
            try
            {
                conn.Open();
                string query = "SELECT o.name AS OwnerName, c.marka AS Marka, c.color AS Color, c.reg_nom AS RegNumber FROM owners as o INNER JOIN cars as c ON o.car_id = c.car_id WHERE name ILIKE @name OR marka ILIKE @marka OR color ILIKE @color OR reg_nom ILIKE @reg_nom";
                var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("name", InfoName.Text);
                cmd.Parameters.AddWithValue("marka", InfoMarka.Text);
                cmd.Parameters.AddWithValue("color", InfoColor.Text);
                cmd.Parameters.AddWithValue("reg_nom", InfoRegNom.Text);
                var reader = cmd.ExecuteReader();
                var info = new List<Info>();
                while (reader.Read())
                {
                    info.Add(new Info
                    {
                        user_name = reader.GetString(0),
                        marka = reader.GetString(1),
                        color = reader.GetString(2),
                        regnom = reader.GetString(3)
                    });
                }
                infoDataGrid.ItemsSource = info;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске: " + ex.Message);
            }
        }
        //end info
    }
}