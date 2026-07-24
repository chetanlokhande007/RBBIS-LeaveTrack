import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { TitleService } from '../../core/services/title.service';
import { DepartmentService } from '../../core/services/department.service';
import { DesignationService } from '../../core/services/designation.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-admin-organization',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './organization.component.html'
})
export class OrganizationComponent implements OnInit {
  departments: any[] = [];
  designations: any[] = [];
  
  activeTab: 'departments' | 'designations' = 'departments';
  
  showModal = false;
  isEdit = false;
  selectedId: number | null = null;
  form: FormGroup;
  submitting = false;

  constructor(
    private fb: FormBuilder,
    private titleService: TitleService,
    private departmentService: DepartmentService,
    private designationService: DesignationService,
    private notification: NotificationService
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      isActive: [true]
    });
  }

  ngOnInit() {
    this.titleService.setTitle('Organization Management');
    this.loadData();
  }

  loadData() {
    if (this.activeTab === 'departments') {
      this.departmentService.getAll().subscribe(res => this.departments = res);
    } else {
      this.designationService.getAll().subscribe(res => this.designations = res);
    }
  }

  setTab(tab: 'departments' | 'designations') {
    this.activeTab = tab;
    this.loadData();
  }

  openAddModal() {
    this.isEdit = false;
    this.selectedId = null;
    this.form.reset({ isActive: true });
    this.showModal = true;
  }

  openEditModal(item: any) {
    this.isEdit = true;
    this.selectedId = item.id;
    this.form.patchValue({
      name: item.name,
      description: item.description,
      isActive: item.isActive
    });
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
  }

  save() {
    if (this.form.invalid) return;
    this.submitting = true;
    const val = this.form.value;
    
    const service = this.activeTab === 'departments' ? this.departmentService : this.designationService;
    const itemName = this.activeTab === 'departments' ? 'Department' : 'Designation';

    if (this.isEdit && this.selectedId) {
      service.update(this.selectedId, val).subscribe({
        next: () => {
          this.submitting = false;
          this.showModal = false;
          this.notification.show(`${itemName} updated successfully`, 'success');
          this.loadData();
        },
        error: () => {
          this.submitting = false;
          this.notification.show(`Failed to update ${itemName.toLowerCase()}`, 'error');
        }
      });
    } else {
      service.create(val).subscribe({
        next: () => {
          this.submitting = false;
          this.showModal = false;
          this.notification.show(`${itemName} created successfully`, 'success');
          this.loadData();
        },
        error: () => {
          this.submitting = false;
          this.notification.show(`Failed to create ${itemName.toLowerCase()}`, 'error');
        }
      });
    }
  }

  deleteItem(id: number) {
    const itemName = this.activeTab === 'departments' ? 'Department' : 'Designation';
    if (confirm(`Are you sure you want to delete this ${itemName.toLowerCase()}?`)) {
      const service = this.activeTab === 'departments' ? this.departmentService : this.designationService;
      service.delete(id).subscribe({
        next: () => {
          this.notification.show(`${itemName} deleted`, 'success');
          this.loadData();
        },
        error: () => this.notification.show(`Failed to delete ${itemName.toLowerCase()}`, 'error')
      });
    }
  }
}
