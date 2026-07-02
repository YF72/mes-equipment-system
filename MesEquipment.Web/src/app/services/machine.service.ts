import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CreateMachine,
  Machine,
  MachineQuery,
  PagedResult,
  UpdateMachine,
} from '../models/machine';

@Injectable({
  providedIn: 'root',
})
export class MachineService {
  private readonly apiUrl = 'http://localhost:5264/api/Machines';

  constructor(private http: HttpClient) {}

  getMachines(query: MachineQuery): Observable<PagedResult<Machine>> {
    return this.http.get<PagedResult<Machine>>(this.apiUrl, {
      params: {
        page: query.page,
        pageSize: query.pageSize,
        ...(query.keyword ? { keyword: query.keyword } : {}),
        ...(query.status ? { status: query.status } : {}),
      },
    });
  }

  createMachine(machine: CreateMachine): Observable<Machine> {
    return this.http.post<Machine>(this.apiUrl, machine);
  }

  updateMachine(id: number, machine: UpdateMachine): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, machine);
  }

  deleteMachine(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
