/**
 * Represents a Data Transfer Object (DTO) for a role.
 * This interface defines the structure of a role object,
 * including its unique identifier, name, optional description,
 * and associated permissions.
 */
export interface IRoleDTO {
  id: string;
  name: string;
  description?: string;
}

