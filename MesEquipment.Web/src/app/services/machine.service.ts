import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { MachinesClient } from '../api/mes-equipment-api';
import {
  toCreateMachineDto,
  toMachine,
  toPagedMachines,
  toUpdateMachineDto,
} from '../adapters/machine-api.mapper';
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
  constructor(private machinesClient: MachinesClient) {}

  getMachines(query: MachineQuery): Observable<PagedResult<Machine>> {
    return this.machinesClient
      .getMachines(query.page, query.pageSize, query.keyword, query.status)
      .pipe(map((result) => toPagedMachines(result, query)));
  }

  getMachine(id: number): Observable<Machine> {
    return this.machinesClient.getMachine(id).pipe(map(toMachine));
  }

  createMachine(machine: CreateMachine): Observable<Machine> {
    return this.machinesClient.createMachine(toCreateMachineDto(machine)).pipe(map(toMachine));
  }

  updateMachine(id: number, machine: UpdateMachine): Observable<void> {
    return this.machinesClient.updateMachine(id, toUpdateMachineDto(machine));
  }

  deleteMachine(id: number): Observable<void> {
    return this.machinesClient.deleteMachine(id);
  }
}
