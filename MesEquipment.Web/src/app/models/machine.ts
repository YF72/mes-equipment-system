export interface Machine {
  id: number;
  code: string;
  name: string;
  location: string;
  status: string;
  createdAt: string;
}

export interface CreateMachine {
  code: string;
  name: string;
  location: string;
  status: string;
}

export interface UpdateMachine {
  code: string;
  name: string;
  location: string;
  status: string;
}

export interface MachineQuery {
  page: number;
  pageSize: number;
  keyword?: string;
  status?: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
