import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { OidcSecurityService, LoginResponse } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgIf],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  
  authentication: LoginResponse;

  constructor(private authService: OidcSecurityService) {
    this.authentication = {
      isAuthenticated: false,
      userData: null,
      accessToken: '',
      idToken: ''
    };
  }

  ngOnInit() {
    var component = this;

    this.authService
      .checkAuth()
      .subscribe((loginResponse: LoginResponse) => {
        component.authentication = loginResponse;
      });
  }

  login() {
    this.authService.authorize();
  }

  logout() {
    this.authService
      .logoff()
      .subscribe((result) => console.log(result));
  }
}
