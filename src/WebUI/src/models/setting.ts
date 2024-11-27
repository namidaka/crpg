export enum SettingDataType {
  Boolean = 'Boolean',
  Number = 'Number',
  String = 'String',
  Json = 'Json',
}

export interface Setting {
  id: number
  key: string
  value: string
  description: string
  private: boolean
  dataType: SettingDataType
  updatedAt: Date
  createdAt: Date
}

export interface SettingEdition extends Omit<Setting, 'id' | 'updatedAt' | 'createdAt'> {}
