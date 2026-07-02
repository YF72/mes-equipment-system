import { createReducer, on } from '@ngrx/store';
import { Machine } from '../../models/machine';
import { MachineActions } from './machine.actions';

export const machineFeatureKey = 'machines';

export interface MachineState {
  machines: Machine[];
  totalCount: number;
  page: number;
  pageSize: number;
  loading: boolean;
  error: string;
}

export const initialMachineState: MachineState = {
  machines: [],
  totalCount: 0,
  page: 1,
  pageSize: 10,
  loading: false,
  error: '',
};

export const machineReducer = createReducer(
  initialMachineState,

  on(MachineActions.loadMachines, (state) => ({
    ...state,
    loading: true,
    error: '',
  })),

  on(MachineActions.loadMachinesSuccess, (state, { result }) => ({
    ...state,
    machines: result.items,
    totalCount: result.totalCount,
    page: result.page,
    pageSize: result.pageSize,
    loading: false,
  })),

  on(MachineActions.loadMachinesFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),
);
