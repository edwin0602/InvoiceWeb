import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { NzMessageService } from 'ng-zorro-antd/message';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css'],
    standalone: false
})
export class LoginComponent {
  validateForm: FormGroup = new FormGroup({
    userName: new FormControl('', [Validators.required]),
    password: new FormControl('', [Validators.required]),
  });

  constructor(private authService: AuthService, private router: Router, private message: NzMessageService) {}

  submitForm(): void {
    if (this.validateForm.valid) {
      const { userName, password } = this.validateForm.value;
      this.authService.login(userName, password).subscribe({
        next: () => this.router.navigate(['/dashboard']),
        error: (err) => {
          // Check if there is an error message from the backend
          if (err.status === 401) {
            // Show the error message from the backend using NzMessageService
            this.message.error(err.error.message || 'Invalid username or password.');
          } else {
            // Handle other error cases
            this.message.error('An unexpected error occurred. Please try again.');
          }
        }
      });
    } else {
      Object.values(this.validateForm.controls).forEach(control => {
        if (control.invalid) {
          control.markAsDirty();
          control.updateValueAndValidity({ onlySelf: true });
        }
      });
    }
  }
}
