export type TaskType =
  | 'Addition'
  | 'Subtraction'
  | 'Comparison'
  | 'MissingNumber'
  | 'NumberSequence'
  | 'Multiplication'
  | 'Division'
  | 'EvenOdd'
  | 'FractionsBasic'
  | 'GeometryBasic'
  | 'WordProblem'

export const TASK_TYPES: TaskType[] = [
  'Addition',
  'Subtraction',
  'Comparison',
  'MissingNumber',
  'NumberSequence',
  'Multiplication',
  'Division',
  'EvenOdd',
  'FractionsBasic',
  'GeometryBasic',
  'WordProblem'
]

export interface StaticExerciseConfig {
  questionTextCs: string
  questionTextEn: string
  correctAnswer: string
  questionData?: Record<string, unknown>
}

export interface BaseTaskConfig {
  minNumber: number
  maxNumber: number
  staticExerciseChancePercent: number
  staticExercises: StaticExerciseConfig[]
}

export interface AdditionConfig extends BaseTaskConfig {
  allowCarry: boolean
  allowNegativeResult: boolean
}

export interface SubtractionConfig extends BaseTaskConfig {
  allowBorrow: boolean
  allowNegativeResult: boolean
}

export interface NumberSequenceConfig extends BaseTaskConfig {
  sequenceStep: number
}

export interface MultiplicationConfig extends BaseTaskConfig {
  multipliers: number[]
}

export interface FractionsBasicConfig extends BaseTaskConfig {
  fractions: string[]
}

/** Comparison, MissingNumber, Division, EvenOdd, GeometryBasic, WordProblem */
export type RangeOnlyConfig = BaseTaskConfig

export type TaskConfigMap = {
  Addition: AdditionConfig
  Subtraction: SubtractionConfig
  Comparison: RangeOnlyConfig
  MissingNumber: RangeOnlyConfig
  NumberSequence: NumberSequenceConfig
  Multiplication: MultiplicationConfig
  Division: RangeOnlyConfig
  EvenOdd: RangeOnlyConfig
  FractionsBasic: FractionsBasicConfig
  GeometryBasic: RangeOnlyConfig
  WordProblem: RangeOnlyConfig
}

export type AnyTaskConfig = TaskConfigMap[TaskType]
