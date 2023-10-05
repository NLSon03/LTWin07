using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmStudent : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        public frmStudent()
        {
            InitializeComponent();
        }

        private bool isNotNullValue()
        {
            if (txtStudentName.Text == "")
                return false;
            if (txtStudentId.Text == "")
                return false;
            if (txtAverageScore.Text == "")
                return false;
            return true;
        }

        private bool isNumberic(string txtID)
        {
            bool checkNumb = long.TryParse(txtID, out long result);
            if (txtID.Contains(","))
                return false;
            return checkNumb;
        }

        private bool isScore(string txtID)
        {
            bool checkNumb = float.TryParse(txtID, out float result);
            if (txtID.Contains(","))
                return false;
            if (checkNumb)
            {
                if (float.Parse(txtID) > 10 || float.Parse(txtID) < 0)
                    return false;
                return true;
            }
            return false;
        }
        private void isValidInputData()
        {
            if (!isNotNullValue())
                throw new Exception("Vui lòng nhập đầy đủ thông tin.");
            if (txtStudentId.Text.Length != 10)
                throw new Exception("Mã số sinh viên phải có 10 kí tự.");
            if (!isNumberic(txtStudentId.Text))
                throw new Exception("Mã số sinh viên không bao gồm chữ.");
            if (!isScore(txtAverageScore.Text))
                throw new Exception("Điểm không hợp lệ.");
        }



        private void btnAdd_Update_Click(object sender, EventArgs e)
        {
            try
            {
                isValidInputData();
                Student student = new Student() { StudentID = txtStudentId.Text, FullName = txtStudentName.Text, FacultyID = Convert.ToInt32(cmbFaculty.SelectedValue), AverageScore = float.Parse(txtAverageScore.Text) };
                studentService.InsertUpdate(student);
                MessageBox.Show("Thêm/Sửa thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.frmStudent_Load(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtStudentId.Text.Length != 10)
                    throw new Exception("Mã số sinh viên phải có 10 kí tự.");
                if (!isNumberic(txtStudentId.Text))
                    throw new Exception("Mã số sinh viên chỉ gồm số.");
                DialogResult dialog = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    studentService.DeleteById(txtStudentId.Text);
                    this.frmStudent_Load(sender, e);
                    MessageBox.Show("Xóa thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var listStudent = new List<Student>();
            if (this.chkUnregisterMajor.Checked)
                listStudent = studentService.GetAllHasNoMajor();
            else
                listStudent = studentService.GetAll();
            BindGrid(listStudent);
        }

        private void BindGrid(List<Student> students)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in students)
            {
                string FacultyName = "";
                string MajorName = "";
                if (item.Faculty != null)
                    FacultyName = item.Faculty.FacultyName;
                if (item.MajorID != null)
                    MajorName = item.Major.Name;
                dgvStudent.Rows.Add(item.StudentID, item.FullName, FacultyName, item.AverageScore + "", MajorName);
                ShowAvatar(item.Avatar);
            }
        }
        private void ShowAvatar(string ImageName)
        {
            if (string.IsNullOrEmpty(ImageName))
            {
                picAvatar.Image = null;
            }
            else
            {
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images",
                ImageName);
                picAvatar.Image = Image.FromFile(imagePath);
                picAvatar.Refresh();
            }
        }
        public void setGridViewStyle(DataGridView dataGridView)
        {
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        private void FillFacultyComboBox(List<Faculty> listFaculties)
        {
            this.cmbFaculty.DataSource = listFaculties;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }
        private void frmStudent_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFaculties = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFacultyComboBox(listFaculties);
                BindGrid(listStudents);
                cmbFaculty.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void menuBtnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn thoát không?", "Thoát?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            String imageLocation = "";
            try
            {
                OpenFileDialog fileOpen = new OpenFileDialog();
                fileOpen.Title = "Chọn hình ảnh sinh viên";
                fileOpen.Filter = "Hình ảnh(*.jpg; *.jpeg; *.png)| *.jpg; *.jpeg; *.png | All files(*.*) | *.* ";
                if (fileOpen.ShowDialog() == DialogResult.OK)
                {
                    imageLocation = fileOpen.FileName;
                    picAvatar.Image = Image.FromFile(imageLocation);
                    //pathImage = imageLocation;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(" Lỗi không thể upload ảnh! ", " Lỗi ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvStudent.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvStudent.SelectedRows[0];
                txtStudentId.Text = row.Cells[0].Value.ToString();
                txtStudentName.Text = row.Cells[1].Value.ToString();
                cmbFaculty.Text = row.Cells[2].Value.ToString();
                txtAverageScore.Text = row.Cells[3].Value.ToString();
                if(studentService.FindById(txtStudentId.Text).Avatar!=null)
                picAvatar.ImageLocation = studentService.FindById(txtStudentId.Text).Avatar.ToString();
                ShowAvatar(picAvatar.ImageLocation);
            }
        }
    }
}
