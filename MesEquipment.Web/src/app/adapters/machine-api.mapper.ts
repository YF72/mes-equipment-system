import {
  CreateMachineDto,
  MachineDto,
  MachineDtoPagedResultDto,
  UpdateMachineDto,
} from '../api/mes-equipment-api';
import {
  CreateMachine,
  Machine,
  MachineQuery,
  PagedResult,
  UpdateMachine,
} from '../models/machine';

export function toCreateMachineDto(machine: CreateMachine): CreateMachineDto {
  return {
    code: machine.code,
    name: machine.name,
    location: machine.location,
    status: machine.status,
  };
}

export function toUpdateMachineDto(machine: UpdateMachine): UpdateMachineDto {
  return {
    code: machine.code,
    name: machine.name,
    location: machine.location,
    status: machine.status,
  };
}

export function toMachine(dto: MachineDto): Machine {
  return {
    id: dto.id!,
    code: dto.code!,
    name: dto.name!,
    location: dto.location!,
    status: dto.status!,
    createdAt: dto.createdAt!,
  };
}

export function toPagedMachines(
  dto: MachineDtoPagedResultDto,
  fallbackQuery: MachineQuery,
): PagedResult<Machine> {
  return {
    items: dto.items?.map(toMachine) ?? [],
    totalCount: dto.totalCount ?? 0,
    page: dto.page ?? fallbackQuery.page,
    pageSize: dto.pageSize ?? fallbackQuery.pageSize,
  };
}
