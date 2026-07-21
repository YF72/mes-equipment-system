import { AsyncPipe, DatePipe } from "@angular/common";
import { Component, computed, inject, OnInit, signal } from "@angular/core";
import { FormBuilder, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Store } from "@ngrx/store";
import { combineLatest, finalize, map } from "rxjs";
import { MachineService } from "../../services/machine.service";
import { MachineActions } from "../../store/machines/machine.actions";
import {
  selectMachines,
  selectMachinesError,
  selectMachinesLoading,
  selectMachinesPage,
  selectMachinesPageSize,
  selectMachinesTotalCount,
} from "../../store/machines/machine.selectors";
import { CreateMachine, Machine, MachineQuery } from "../../models/machine";
import { AuthService } from "../../services/auth.service";
import { AppRoles } from "../../models/auth";

@Component({
  selector: "app-machine-list",
  imports: [AsyncPipe, DatePipe, FormsModule, ReactiveFormsModule],
  templateUrl: "./machine-list.html",
  styleUrl: "./machine-list.css",
})
export class MachineListComponent implements OnInit {
  private store = inject(Store);
  private fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);

  protected readonly canCreateMachine = computed(() =>
    this.authService.hasAnyRole(
      AppRoles.administrator,
      AppRoles.equipmentEngineer,
    ),
  );

  protected readonly canUpdateMachine = computed(() =>
    this.authService.hasAnyRole(
      AppRoles.administrator,
      AppRoles.equipmentEngineer,
      AppRoles.engineering,
      AppRoles.processIntegrationEngineer,
    ),
  );

  protected readonly canDeleteMachine = computed(() =>
    this.authService.hasAnyRole(
      AppRoles.administrator,
      AppRoles.equipmentEngineer,
    ),
  );
  machines$ = this.store.select(selectMachines);
  loading$ = this.store.select(selectMachinesLoading);
  errorMessage$ = this.store.select(selectMachinesError);

  operationError = signal("");
  successMessage = signal("");
  deletingId = signal<number | null>(null);
  saving = signal(false);
  editingId = signal<number | null>(null);

  form: CreateMachine = this.getEmptyForm();

  filterForm = this.fb.nonNullable.group({
    keyword: "",
    status: "",
    pageSize: 10,
  });

  page = 1;

  totalCount$ = this.store.select(selectMachinesTotalCount);
  page$ = this.store.select(selectMachinesPage);
  pageSize$ = this.store.select(selectMachinesPageSize);

  pagination$ = combineLatest([
    this.totalCount$,
    this.page$,
    this.pageSize$,
  ]).pipe(
    map(([totalCount, page, pageSize]) => ({
      totalCount,
      page,
      pageSize,
      totalPages: Math.max(1, Math.ceil(totalCount / pageSize)),
    })),
  );

  constructor(private machineService: MachineService) {}

  ngOnInit(): void {
    this.machines$ = this.store.select(selectMachines);
    this.loading$ = this.store.select(selectMachinesLoading);
    this.errorMessage$ = this.store.select(selectMachinesError);
    this.loadMachines();
  }

  loadMachines(): void {
    this.store.dispatch(
      MachineActions.loadMachines({
        query: this.buildQuery(),
      }),
    );
  }

  applyFilters(): void {
    this.page = 1;
    this.loadMachines();
  }

  clearFilters(): void {
    this.filterForm.reset({
      keyword: "",
      status: "",
      pageSize: this.filterForm.controls.pageSize.value,
    });

    this.page = 1;
    this.loadMachines();
  }

  goToPreviousPage(currentPage: number): void {
    if (currentPage <= 1) {
      return;
    }

    this.page = currentPage - 1;
    this.loadMachines();
  }

  goToNextPage(currentPage: number, totalPages: number): void {
    if (currentPage >= totalPages) {
      return;
    }

    this.page = currentPage + 1;
    this.loadMachines();
  }

  changePageSize(pageSize: number): void {
    this.filterForm.patchValue({ pageSize });

    this.page = 1;
    this.loadMachines();
  }

  saveMachine(): void {
    this.clearMessages();
    this.saving.set(true);

    if (this.editingId() === null) {
      this.machineService
        .createMachine(this.form)
        .pipe(finalize(() => this.saving.set(false)))
        .subscribe({
          next: () => {
            this.resetForm();
            this.successMessage.set("Machine created successfully.");
            this.loadMachines();
          },
          error: (error) => {
            console.error("Failed to create machine:", error);
            this.operationError.set(
              "Failed to create machine. Please check the form and try again.",
            );
          },
        });

      return;
    }

    this.machineService
      .updateMachine(this.editingId()!, this.form)
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.resetForm();
          this.successMessage.set("Machine created successfully.");
          this.loadMachines();
        },
        error: (error) => {
          console.error("Failed to update machine:", error);
          this.operationError.set(
            "Failed to update machine. Please check the form and try again.",
          );
        },
      });
  }

  editMachine(machine: Machine): void {
    this.editingId.set(machine.id);
    this.form = {
      code: machine.code,
      name: machine.name,
      location: machine.location,
      status: machine.status,
    };
  }

  deleteMachine(machine: Machine): void {
    const confirmed = confirm(`Delete machine ${machine.code}?`);

    if (!confirmed) {
      return;
    }

    this.clearMessages();
    this.machineService
      .deleteMachine(machine.id)
      .pipe(finalize(() => this.deletingId.set(null)))
      .subscribe({
        next: () => {
          this.successMessage.set(
            `Machine ${machine.code} deleted successfully.`,
          );
          this.loadMachines();
        },
        error: (error) => {
          console.error("Failed to delete machine:", error);
          this.operationError.set(
            `Failed to delete machine ${machine.code}. Please try again.`,
          );
        },
      });
  }

  resetForm(): void {
    this.editingId.set(null);
    this.form = this.getEmptyForm();
  }

  private getEmptyForm(): CreateMachine {
    return {
      code: "",
      name: "",
      location: "",
      status: "Idle",
    };
  }

  private buildQuery(): MachineQuery {
    const formValue = this.filterForm.getRawValue();

    return {
      page: this.page,
      pageSize: formValue.pageSize,
      keyword: formValue.keyword.trim() || undefined,
      status: formValue.status || undefined,
    };
  }

  private clearMessages(): void {
    this.operationError.set("");
    this.successMessage.set("");
  }
}
