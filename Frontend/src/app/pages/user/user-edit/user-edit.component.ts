import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  FormControl,
  FormGroup,
  NonNullableFormBuilder,
  Validators,
} from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { RoleService } from 'src/app/services/role.service';
import { NzMessageService } from 'ng-zorro-antd/message';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.less'],
})
export class UserEditComponent implements OnInit {
  editUserForm: FormGroup;
  userId!: string;
  roles: any[] = [];
  selectedFile: File | null = null;
  selectedFilePreview: string | ArrayBuffer | null = null;
  photoUrl: string | null = null;
  loading = true;
  basePath: string;

  constructor(
    private fb: NonNullableFormBuilder,
    private userService: UserService,
    private roleService: RoleService,
    private router: Router,
    private route: ActivatedRoute,
    private message: NzMessageService
  ) {
    this.editUserForm = this.fb.group({
      userId: new FormControl<string | null>({ value: null, disabled: true }),
      username: new FormControl<string | null>(null, [Validators.required]),
      emailAddress: new FormControl<string | null>(null, [
        Validators.required,
        Validators.email,
      ]),
      firstName: new FormControl<string | null>(null),
      lastName: new FormControl<string | null>(null),
      documentNumber: new FormControl<string | null>(null),
      address: new FormControl<string | null>(null),
      phoneNumber: new FormControl<string | null>(null),
      role_id: new FormControl<string | null>(null, [Validators.required]),
      status: new FormControl<string | null>(null),
      photoUrl: new FormControl<string | null>(null),
      passwordHash: new FormControl<string | null>(null),
      creationDate: new FormControl<string | null>(
        { value: null, disabled: true }
      ),
      updateDate: new FormControl<string | null>(
        { value: null, disabled: true }
      ),
    });
    this.basePath = environment.apiUrl.replace('/api/', '');
  }

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('userId')!;
    this.loadRoles();
  }

  loadRoles(): void {
    this.roleService.getRoles().subscribe(
      (roles) => {
        this.roles = roles;
        this.loadUser();
      },
      (error) => {
        console.error('Error fetching roles', error);
        this.loading = false;
      }
    );
  }

  loadUser(): void {
    this.userService.getUserById(this.userId).subscribe(
      (user) => {
        this.editUserForm.patchValue({
          userId: user.userId,
          username: user.username,
          emailAddress: user.emailAddress,
          firstName: user.firstName,
          lastName: user.lastName,
          documentNumber: user.documentNumber,
          address: user.address,
          phoneNumber: user.phoneNumber,
          role_id: user.role_id,
          status: user.status,
          photoUrl: user.photoUrl,
          passwordHash: '', // Por seguridad, no mostrar el hash real
          creationDate: user.creationDate,
          updateDate: user.updateDate,
        });
        this.photoUrl = user.photoUrl;
        this.loading = false;
      },
      (error) => {
        this.message.error('Error loading user details');
        this.loading = false;
      }
    );
  }

  onFileChange(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.selectedFilePreview = e.target.result;
      };
      reader.readAsDataURL(file);
    } else {
      this.selectedFile = null;
      this.selectedFilePreview = null;
    }
  }

  submitForm(): void {
    if (this.editUserForm.valid) {
      const formData = new FormData();
      // Incluye todos los campos, incluso los deshabilitados
      Object.keys(this.editUserForm.controls).forEach((key) => {
        const control = this.editUserForm.get(key);
        if (control && control.value !== null && control.value !== undefined) {
          formData.append(key, String(control.value));
        }
      });

      if (this.selectedFile) {
        formData.append('photo', this.selectedFile);
      }

      this.userService.updateUser(this.userId, formData).subscribe({
        next: () => {
          this.message.success('Usuario actualizado correctamente');
          this.router.navigate(['/users/all']);
        },
        error: (err) => {
          this.message.error('Error actualizando usuario');
          console.error('Error updating user:', err);
        },
      });
    }
  }

  resetForm(e: MouseEvent): void {
    e.preventDefault();
    this.editUserForm.reset();
  }

  onBack(): void {
    this.router.navigate(['/users/all']);
  }
}
