export const AppRoles = {
  administrator: "Administrator",
  equipmentEngineer: "EquipmentEngineer",
  equipmentManager: "EquipmentManager",
  qualityEngineer: "QualityEngineer",
  engineering: "Engineering",
  processIntegrationEngineer: "ProcessIntegrationEngineer",
} as const;

export type AppRole = (typeof AppRoles)[keyof typeof AppRoles];
