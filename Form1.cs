using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Lab7_OOAP_
{
    // Головна форма програми
    public partial class Form1 : Form
    {
        private Accounting accounting; // Об'єкт класу Accounting для обробки списку працівників

        // Конструктор форми
        public Form1()
        {
            InitializeCustomComponent(); // Ініціалізація елементів форми
            accounting = new Accounting(); // Ініціалізація об'єкта Accounting

            // Встановлення розмірів форми
            Height = 500; Width = 550;
        }

        // Обробник події натискання кнопки для додавання працівника
        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            // Отримання введеного імені, посади та форми оплати
            string name = txtName.Text;
            string position = cmbPosition.SelectedItem?.ToString();
            string paymentType = cmbPaymentType.SelectedItem?.ToString();

            // Перевірка наявності введеного імені
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введіть ім'я працівника!");
                return;
            }
            // Перевірка обраної форми оплати
            if (string.IsNullOrWhiteSpace(paymentType))
            {
                MessageBox.Show("Оберіть тип оплати!");
                return;
            }

            // Вибір типу розрахунку зарплати на основі форми оплати
            ISalaryCalculator salaryCalculator;
            if (paymentType == "Відрядна форма") // Якщо відрядна форма оплати
            {
                // Перевірка та конвертація введених значень
                if (!decimal.TryParse(txtRate.Text, out decimal ratePerPiece) || !int.TryParse(txtPieces.Text, out int piecesCompleted))
                {
                    MessageBox.Show("Введіть коректні дані для ставки або кількості деталей!");
                    return;
                }
                salaryCalculator = new PieceworkSalaryCalculator(ratePerPiece, piecesCompleted); // Ініціалізація калькулятора відрядної зарплати
            }
            else // Якщо погодинна форма оплати
            {
                if (!decimal.TryParse(txtRate.Text, out decimal hourlyRate) || !int.TryParse(txtHours.Text, out int hoursWorked))
                {
                    MessageBox.Show("Введіть коректні дані для погодинної ставки або кількості годин!");
                    return;
                }
                salaryCalculator = new HourlySalaryCalculator(hourlyRate, hoursWorked); // Ініціалізація калькулятора погодинної зарплати
            }

            // Ініціалізація об'єкта Employee відповідного типу
            Employee employee;
            switch (position)
            {
                case "Менеджер":
                    employee = new Manager(name, salaryCalculator); // Створення об'єкта класу Manager
                    break;
                case "Інженер":
                    employee = new Engineer(name, salaryCalculator); // Створення об'єкта класу Engineer
                    break;
                case "Робітник":
                    employee = new Worker(name, salaryCalculator); // Створення об'єкта класу Worker
                    break;
                default:
                    MessageBox.Show("Оберіть посаду!");
                    return;
            }

            // Додавання працівника до об'єкта Accounting
            accounting.AddEmployee(employee);
            lstEmployees.Items.Clear(); // Очищення списку працівників
            foreach (var employeee in accounting.GetEmployees()) // Виведення списку працівників
            {
                lstEmployees.Items.Add($"{employeee.Position}: {employeee.Name}");
            }
        }

        // Відображення зарплати обраного працівника при зміні вибору в списку
        private void lstEmployees_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstEmployees.SelectedIndex >= 0)
            {
                var employee = accounting.GetEmployees()[lstEmployees.SelectedIndex];
                lblSalary.Text = $"Зарплата: {employee.GetSalary()} грн"; // Виклик методу GetSalary() через шаблон Делегування
            }
        }

        // Ініціалізація компонентів форми
        private void InitializeCustomComponent()
        {
            // Ініціалізація та додавання всіх елементів управління
            lblName = new Label { Text = "Ім'я працівника", Location = new Point(10, 10), Width = 150 };
            Controls.Add(lblName);

            txtName = new TextBox { Location = new Point(160, 10), Width = 150 };
            Controls.Add(txtName);

            lblPosition = new Label { Text = "Посада працівника", Location = new Point(10, 40), Width = 150 };
            Controls.Add(lblPosition);

            cmbPosition = new ComboBox { Location = new Point(160, 40), Width = 150, Items = { "Менеджер", "Інженер", "Робітник" } };
            Controls.Add(cmbPosition);

            lblPaymentType = new Label { Text = "Форма оплати", Location = new Point(10, 70), Width = 150 };
            Controls.Add(lblPaymentType);

            cmbPaymentType = new ComboBox { Location = new Point(160, 70), Width = 150, Items = { "Відрядна форма", "Погодинна форма" } };
            Controls.Add(cmbPaymentType);

            lblRate = new Label { Text = "Ставка робітника", Location = new Point(10, 100), Width = 150 };
            Controls.Add(lblRate);

            txtRate = new TextBox { Location = new Point(160, 100), Width = 150 };
            Controls.Add(txtRate);

            lblPieces = new Label { Text = "Скільки виготовлено", Location = new Point(10, 130), Width = 150 };
            Controls.Add(lblPieces);

            txtPieces = new TextBox { Location = new Point(160, 130), Width = 150 };
            Controls.Add(txtPieces);

            lblHours = new Label { Text = "Скільки відпрацьовано", Location = new Point(10, 160), Width = 150 };
            Controls.Add(lblHours);

            txtHours = new TextBox { Location = new Point(160, 160), Width = 150 };
            Controls.Add(txtHours);

            btnAddEmployee = new Button { Location = new Point(10, 190), Width = 150, Text = "Додати працівника" };
            btnAddEmployee.Click += new EventHandler(btnAddEmployee_Click);
            Controls.Add(btnAddEmployee);

            lstEmployees = new ListBox { Location = new Point(320, 10), Width = 200, Height = 250 };
            lstEmployees.SelectedIndexChanged += new EventHandler(lstEmployees_SelectedIndexChanged);
            Controls.Add(lstEmployees);

            lblSalary = new Label { Location = new Point(320, 270), Width = 200, Height = 30, Text = "Зарплата: " };
            Controls.Add(lblSalary);

            this.Text = "Бухгалтерія";
        }

        // Декларації елементів управління
        private Label lblName;
        private TextBox txtName;
        private Label lblPosition;
        private ComboBox cmbPosition;
        private Label lblPaymentType;
        private ComboBox cmbPaymentType;
        private Label lblRate;
        private TextBox txtRate;
        private Label lblPieces;
        private TextBox txtPieces;
        private Label lblHours;
        private TextBox txtHours;
        private Button btnAddEmployee;
        private ListBox lstEmployees;
        private Label lblSalary;
    }

    // Клас Accounting для управління списком працівників
    public class Accounting
    {
        private List<Employee> employees = new List<Employee>();

        public void AddEmployee(Employee employee)
        {
            employees.Add(employee); // Додавання працівника до списку
        }

        public List<Employee> GetEmployees()
        {
            return employees; // Повернення списку працівників
        }
    }

    // Інтерфейс ISalaryCalculator — абстрагує метод розрахунку зарплати
    public interface ISalaryCalculator
    {
        decimal CalculateSalary();
    }

    // Клас PieceworkSalaryCalculator для відрядної оплати праці
    public class PieceworkSalaryCalculator : ISalaryCalculator
    {
        private decimal ratePerPiece; // Ставка за деталь
        private int piecesCompleted; // Кількість виготовлених деталей

        public PieceworkSalaryCalculator(decimal ratePerPiece, int piecesCompleted)
        {
            this.ratePerPiece = ratePerPiece;
            this.piecesCompleted = piecesCompleted;
        }

        public decimal CalculateSalary()
        {
            return ratePerPiece * piecesCompleted; // Обчислення зарплати
        }
    }

    // Клас HourlySalaryCalculator для погодинної оплати праці
    public class HourlySalaryCalculator : ISalaryCalculator
    {
        private decimal hourlyRate; // Погодинна ставка
        private int hoursWorked; // Кількість відпрацьованих годин

        public HourlySalaryCalculator(decimal hourlyRate, int hoursWorked)
        {
            this.hourlyRate = hourlyRate;
            this.hoursWorked = hoursWorked;
        }

        public decimal CalculateSalary()
        {
            return hourlyRate * hoursWorked; // Обчислення зарплати
        }
    }

    // Абстрактний клас Employee для працівників
    public abstract class Employee
    {
        protected ISalaryCalculator salaryCalculator; // Поле для розрахунку зарплати

        public string Name { get; } // Ім'я працівника
        public string Position { get; } // Посада працівника

        public Employee(string name, string position, ISalaryCalculator salaryCalculator)
        {
            Name = name;
            Position = position;
            this.salaryCalculator = salaryCalculator;
        }

        public decimal GetSalary()
        {
            return salaryCalculator.CalculateSalary(); // Виклик розрахунку зарплати через шаблон Делегування
        }
    }

    // Клас Manager, що успадковує Employee
    public class Manager : Employee
    {
        public Manager(string name, ISalaryCalculator salaryCalculator) : base(name, "Менеджер", salaryCalculator) { }
    }

    // Клас Engineer, що успадковує Employee
    public class Engineer : Employee
    {
        public Engineer(string name, ISalaryCalculator salaryCalculator) : base(name, "Інженер", salaryCalculator) { }
    }

    // Клас Worker, що успадковує Employee
    public class Worker : Employee
    {
        public Worker(string name, ISalaryCalculator salaryCalculator) : base(name, "Робітник", salaryCalculator) { }
    }
}