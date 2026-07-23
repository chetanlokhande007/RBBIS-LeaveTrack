import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TitleService } from '../../core/services/title.service';
import { EmployeeService } from '../../core/services/employee.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-admin-employees',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './employees.component.html'
})
export class EmployeesComponent implements OnInit {
  employees: any[] = [];
  filteredEmployees: any[] = [];
  searchQuery = '';
  
  showModal = false;
  isEdit = false;
  selectedEmployeeId: number | null = null;
  employeeForm: FormGroup;
  submitting = false;

  constructor(
    private fb: FormBuilder,
    private titleService: TitleService,
    private employeeService: EmployeeService,
    private notification: NotificationService
  ) {
    this.employeeForm = this.fb.group({
      username: [''],
      password: [''],
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      department: ['', Validators.required],
      designation: ['', Validators.required],
      managerId: [''],
      dateOfJoining: ['', Validators.required],
      roleId: ['3'] // Default to Employee (3). Manager is (2). Admin is (1).
    });
  }

  ngOnInit() {
    this.titleService.setTitle('Manage Employees');
    this.loadEmployees();
  }

  loadEmployees() {
    this.employeeService.getAll().subscribe({
      next: (res) => {
        this.employees = res;
        this.applyFilters();
      }
    });
  }

  applyFilters() {
    if (!this.searchQuery) {
      this.filteredEmployees = this.employees;
      return;
    }
    const q = this.searchQuery.toLowerCase();
    this.filteredEmployees = this.employees.filter(e => 
      e.fullName.toLowerCase().includes(q) ||
      e.department.toLowerCase().includes(q) ||
      e.designation.toLowerCase().includes(q) ||
      e.email.toLowerCase().includes(q)
    );
  }

  openAddModal() {
    this.isEdit = false;
    this.selectedEmployeeId = null;
    this.employeeForm.reset({ roleId: '3' });
    // Backend will auto-generate username and password

    this.showModal = true;
  }

  openEditModal(emp: any) {
    this.isEdit = true;
    this.selectedEmployeeId = emp.id;
    this.employeeForm.reset();
    // Backend auto-generates username/password on add

    this.employeeForm.patchValue({
      fullName: emp.fullName,
      email: emp.email,
      department: emp.department,
      designation: emp.designation,
      managerId: emp.managerId || '',
      dateOfJoining: emp.dateOfJoining
    });

    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  saveEmployee() {
    if (this.employeeForm.invalid) return;

    this.submitting = true;
    const val = this.employeeForm.value;

    if (this.isEdit && this.selectedEmployeeId) {
      // Edit mode: Update employee details
      const payload = {
        fullName: val.fullName,
        email: val.email,
        department: val.department,
        designation: val.designation,
        managerId: val.managerId ? Number(val.managerId) : null,
        dateOfJoining: val.dateOfJoining
      };

      this.employeeService.update(this.selectedEmployeeId, payload).subscribe({
        next: () => {
          this.submitting = false;
          this.showModal = false;
          this.notification.show('Employee profile updated successfully', 'success');
          this.loadEmployees();
        },
        error: (err) => {
          this.submitting = false;
          this.notification.show(err.error || 'Failed to update employee', 'error');
        }
      });
    } else {
      // Add mode: Register user + employee
      if (!val.username || !val.password) {
        this.notification.show('Username and password are required!', 'error');
        return;
      }
      const payload = {
        username: val.username,
        password: val.password,
        roleId: Number(val.roleId),
        employeeDetails: {
          fullName: val.fullName,
          email: val.email,
          department: val.department,
          designation: val.designation,
          managerId: val.managerId ? Number(val.managerId) : null,
          dateOfJoining: val.dateOfJoining
        }
      };

      this.employeeService.registerUser(payload).subscribe({
        next: () => {
          this.submitting = false;
          this.showModal = false;
          this.notification.show('New employee registered successfully!', 'success');
          this.loadEmployees();
        },
        error: (err) => {
          this.submitting = false;
          this.notification.show(err.error || 'Registration failed', 'error');
        }
      });
    }
  }

  deleteEmployee(id: number) {
    if (confirm('Are you sure you want to remove this employee? This will delete all attendance and leave history.')) {
      this.employeeService.delete(id).subscribe({
        next: () => {
          this.notification.show('Employee profile removed', 'success');
          this.loadEmployees();
        },
        error: () => this.notification.show('Failed to remove employee', 'error')
      });
    }
  }
}
