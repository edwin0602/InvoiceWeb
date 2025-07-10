import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less'],
  standalone: false
})
export class AppComponent implements OnInit, OnDestroy {
  isCollapsed = false;
  username: string | null = null;
  userInitials: string | null = null;
  userId: string | null = null;
  role: string | null = null;
  private authSubscription: Subscription | null = null;

  menuItems = [
    { label: 'Inicio', icon: 'dashboard', route: '/dashboard' },
    { label: 'Facturas', icon: 'send', route: '/invoices/all' },
    { label: 'Clientes', icon: 'team', route: '/customers/all' },
    { label: 'Productos', icon: 'folder-open', route: '/items/all' },
    {
      label: 'Configuración',
      icon: 'setting',
      children: [
        { label: 'IVA', route: '/vat/all' },
        { label: 'Usuarios', route: '/users/all', adminOnly: true },
        { label: 'Roles', route: '/roles/all', adminOnly: true }
      ]
    }
  ];

  constructor(public authService: AuthService) { }


  ngOnInit(): void {
    this.updateUserInfo();
    this.authSubscription = this.authService.userInfoUpdated.subscribe(() => {
      this.updateUserInfo();
    });
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  private updateUserInfo(): void {
    const userInfo = this.authService.getUserInfo();
    this.username = userInfo.username;
    this.userId = userInfo.userId;
    this.role = userInfo.role;
    this.userInitials = userInfo.username
      ? userInfo.username.substring(0, 2).toUpperCase()
      : '';
  }

  closeMenuOnMobile() {
    if (window.innerWidth < 992) {
      this.isCollapsed = true;
    }
  }
}
