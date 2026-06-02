import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@legal-ai-ar/core';

const BASE = `${environment.apiUrl}/api/admin/users`;

export type UserRole = 'admin' | 'lawyer' | 'viewer';

export interface User {
  id: string;
  email: string;
  displayName: string | null;
  role: UserRole;
  isActive: boolean;
}

export interface CreateUserRequest {
  email: string;
  displayName?: string;
  role: UserRole;
}

export interface UpdateUserRequest {
  displayName?: string;
  role: UserRole;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private http: HttpClient) {}

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(BASE);
  }

  createUser(request: CreateUserRequest): Observable<User> {
    return this.http.post<User>(BASE, request);
  }

  updateUser(id: string, request: UpdateUserRequest): Observable<User> {
    return this.http.put<User>(`${BASE}/${id}`, request);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${BASE}/${id}`);
  }
}
