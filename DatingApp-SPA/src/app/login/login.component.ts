import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  constructor(
    private authService: AuthService,
    private alertService: AlertifyService,
    private router: Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(url => this.photoUrl = url)
  }

  get loggedIn(): boolean {
    return this.authService.loggedIn();
  }

  get userName(): string {

    const res = this.authService.decodedToken;
    if (res) {
      return res.unique_name as string;
    }
    return '';
  }

  login() {
    this.authService.login(this.model).subscribe(
      next => {
        this.alertService.success('Logged in successfully');
        this.router.navigate(['/members']);
      },
      error => {
        this.alertService.error(error);
      }
    );
  }


  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertService.message('logged out');
    this.router.navigate(['/members']);
  }
}
