import { HttpClient } from "@angular/common/http";
import { Injectable, signal } from "@angular/core";
import { Observable, tap } from "rxjs";
import { AppRole } from "../models/auth";

interface LoginRequest {
  username: string;
  password: string;
}

interface LoginResponse {
  token: string;
  username: string;
  role: AppRole;
}

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private readonly apiUrl = "http://localhost:5264/api/Auth";
  private readonly tokenKey = "mes_token";
  private readonly roleKey = "mes_role";

  private readonly loggedInState = signal(this.hasToken());
  private readonly roleState = signal<AppRole | null>(this.getStoredRole());

  readonly isLoggedIn = this.loggedInState.asReadonly();
  readonly currentRole = this.roleState.asReadonly();

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap((response) => {
        localStorage.setItem(this.tokenKey, response.token);
        localStorage.setItem(this.roleKey, response.role);

        this.loggedInState.set(true);
        this.roleState.set(response.role);
      }),
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.roleKey);

    this.loggedInState.set(false);
    this.roleState.set(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    return this.getToken() !== null;
  }

  private hasToken(): boolean {
    return localStorage.getItem(this.tokenKey) !== null;
  }

  hasAnyRole(...roles: AppRole[]): boolean {
    const currentRole = this.roleState();

    return currentRole !== null && roles.includes(currentRole);
  }

  private getStoredRole(): AppRole | null {
    return localStorage.getItem(this.roleKey) as AppRole | null;
  }
}
