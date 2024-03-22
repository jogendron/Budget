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
  
  authentification: LoginResponse;

  constructor(private authService: OidcSecurityService) {
    this.authentification = {
      isAuthenticated: false,
      userData: null,
      accessToken: '',
      idToken: ''
    };
  }

  ngOnInit() {
    var composant = this;

    this.authService
      .checkAuth()
      .subscribe((loginResponse: LoginResponse) => {
        composant.authentification = loginResponse;
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
