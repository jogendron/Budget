import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouterOutlet , RouterLink, RouterLinkActive} from '@angular/router';
import { OidcSecurityService, LoginResponse } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [NgIf, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  
  authentication: LoginResponse;

  constructor(private securityService: OidcSecurityService) {
    this.authentication = {
      isAuthenticated: false,
      userData: null,
      accessToken: '',
      idToken: ''
    };
  }

  ngOnInit() {
    var component = this;

    this.securityService
      .checkAuth()
      .subscribe((loginResponse: LoginResponse) => {
        component.authentication = loginResponse;
      });
  }

  login() {
    this.securityService.authorize();
  }

  logout() {
    this.securityService
      .logoff()
      .subscribe((result) => console.log(result));
  }
}
