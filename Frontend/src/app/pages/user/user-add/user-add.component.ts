import { Component } from '@angular/core';
import { Router } from '@angular/router';
import {
  FormControl,
  FormGroup,
  NonNullableFormBuilder,
  Validators,
} from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { UserService } from 'src/app/services/user.service';
import { RoleService } from 'src/app/services/role.service';

@Component({
  selector: 'app-user-add',
  templateUrl: './user-add.component.html',
  styleUrls: ['./user-add.component.less'],
})
export class UserAddComponent {
  addUserForm: FormGroup;
  roles: any[] = [];
  selectedFile: File | null = null;
  selectedFilePreview: string | ArrayBuffer | null = null;

  constructor(
    private fb: NonNullableFormBuilder,
    private userService: UserService,
    private roleService: RoleService,
    private router: Router,
    private message: NzMessageService
  ) {
    this.addUserForm = this.fb.group({
      username: new FormControl<string | null>(null, [Validators.required]),
      emailAddress: new FormControl<string | null>(null, [Validators.required, Validators.email]),
      firstName: new FormControl<string | null>(null),
      lastName: new FormControl<string | null>(null),
      documentNumber: new FormControl<string | null>(null),
      address: new FormControl<string | null>(null),
      phoneNumber: new FormControl<string | null>(null),
      passwordHash: new FormControl<string | null>(null, [Validators.required]),
      role_id: new FormControl<string | null>(null, [Validators.required]),
      photoUrl: new FormControl<string | null>(null),
    });
  }

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(): void {
    this.roleService.getRoles().subscribe({
      next: (roles) => {
        this.roles = roles;
      },
      error: (err) => {
        this.message.error('Error al cargar los roles');
        console.error('Error al cargar los roles:', err);
      },
    });
  }

  submitForm(): void {
    if (this.addUserForm.valid) {
      const formData = new FormData();
      Object.entries(this.addUserForm.value).forEach(([key, value]) => {
        if (value !== null && value !== undefined) {
          formData.append(key, value !== null && value !== undefined ? String(value) : '');
        }
      });
      if (this.selectedFile) {
        formData.append('photo', this.selectedFile);
      }

      this.userService.addUser(formData).subscribe({
        next: () => {
          this.message.success('Usuario agregado exitosamente');
          this.router.navigate(['/users/all']);
        },
        error: (err) => {
          this.message.error('Error al agregar usuario');
          console.error('Error al agregar usuario:', err);
        },
      });
    }
  }

  resetForm(e: MouseEvent): void {
    e.preventDefault();
    this.addUserForm.reset();
    this.selectedFile = null;
    this.selectedFilePreview = null;
  }

  onFileChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = e => this.selectedFilePreview = reader.result;
      reader.readAsDataURL(file);
    }
  }

  onBack(): void {
    this.router.navigate(['/users/all']);
  }
}
