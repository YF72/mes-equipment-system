import { createFeatureSelector, createSelector } from '@ngrx/store';
import { machineFeatureKey, MachineState } from './machine.reducer';

export const selectMachineState = createFeatureSelector<MachineState>(machineFeatureKey);

export const selectMachines = createSelector(selectMachineState, (state) => state.machines);

export const selectMachinesLoading = createSelector(selectMachineState, (state) => state.loading);

export const selectMachinesError = createSelector(selectMachineState, (state) => state.error);

export const selectMachinesTotalCount = createSelector(
  selectMachineState,
  (state) => state.totalCount,
);

export const selectMachinesPage = createSelector(selectMachineState, (state) => state.page);

export const selectMachinesPageSize = createSelector(selectMachineState, (state) => state.pageSize);
