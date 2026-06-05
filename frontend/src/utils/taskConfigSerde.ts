import type {
  AdditionConfig,
  AnyTaskConfig,
  BaseTaskConfig,
  FractionsBasicConfig,
  MultiplicationConfig,
  NumberSequenceConfig,
  StaticExerciseConfig,
  SubtractionConfig,
  TaskConfigMap,
  TaskType
} from '../types/taskConfig'

type RawConfig = Record<string, unknown>

function num(value: unknown, fallback: number): number {
  return typeof value === 'number' && !Number.isNaN(value) ? value : fallback
}

function bool(value: unknown, fallback: boolean): boolean {
  return typeof value === 'boolean' ? value : fallback
}

function parseStaticExercises(raw: unknown): StaticExerciseConfig[] {
  if (!Array.isArray(raw)) return []
  return raw.map((item) => {
    const x = item as Record<string, unknown>
    const questionData =
      x.questionData && typeof x.questionData === 'object' && !Array.isArray(x.questionData)
        ? (x.questionData as Record<string, unknown>)
        : undefined
    return {
      questionTextCs: String(x.questionTextCs ?? ''),
      questionTextEn: String(x.questionTextEn ?? ''),
      correctAnswer: String(x.correctAnswer ?? ''),
      questionData
    }
  })
}

function parseBase(raw: RawConfig, defaults: BaseTaskConfig): BaseTaskConfig {
  return {
    minNumber: num(raw.minNumber, defaults.minNumber),
    maxNumber: num(raw.maxNumber, defaults.maxNumber),
    staticExerciseChancePercent: num(raw.staticExerciseChancePercent, defaults.staticExerciseChancePercent),
    staticExercises: parseStaticExercises(raw.staticExercises)
  }
}

function parseNumberArray(value: unknown, fallback: number[]): number[] {
  if (!Array.isArray(value)) return [...fallback]
  const nums = value.map((v) => Number(v)).filter((n) => !Number.isNaN(n))
  return nums.length ? nums : [...fallback]
}

function parseStringArray(value: unknown, fallback: string[]): string[] {
  if (!Array.isArray(value)) return [...fallback]
  const items = value.map((v) => String(v).trim()).filter(Boolean)
  return items.length ? items : [...fallback]
}

export function defaultTaskConfig(taskType: TaskType): AnyTaskConfig {
  const base: BaseTaskConfig = {
    minNumber: 0,
    maxNumber: 10,
    staticExerciseChancePercent: 0,
    staticExercises: []
  }

  switch (taskType) {
    case 'Addition':
      return { ...base, allowCarry: true, allowNegativeResult: false } satisfies AdditionConfig
    case 'Subtraction':
      return { ...base, allowBorrow: true, allowNegativeResult: false } satisfies SubtractionConfig
    case 'NumberSequence':
      return { ...base, minNumber: 0, maxNumber: 50, sequenceStep: 2 } satisfies NumberSequenceConfig
    case 'Multiplication':
      return {
        ...base,
        minNumber: 1,
        maxNumber: 10,
        multipliers: [2, 5, 10]
      } satisfies MultiplicationConfig
    case 'FractionsBasic':
      return {
        ...base,
        minNumber: 1,
        maxNumber: 12,
        fractions: ['1/2', '1/3', '1/4']
      } satisfies FractionsBasicConfig
    case 'Division':
      return { ...base, minNumber: 1, maxNumber: 10 }
    case 'EvenOdd':
      return { ...base, minNumber: 0, maxNumber: 100 }
    case 'GeometryBasic':
      return { ...base, minNumber: 0, maxNumber: 0 }
  }

  return { ...base }
}

export function parseTaskConfig(taskType: TaskType, json: string): AnyTaskConfig {
  const defaults = defaultTaskConfig(taskType)
  let raw: RawConfig = {}
  try {
    raw = JSON.parse(json || '{}') as RawConfig
  } catch {
    return defaults
  }

  const base = parseBase(raw, defaults)

  switch (taskType) {
    case 'Addition': {
      const d = defaults as AdditionConfig
      return {
        ...base,
        allowCarry: bool(raw.allowCarry, d.allowCarry),
        allowNegativeResult: bool(raw.allowNegativeResult, d.allowNegativeResult)
      }
    }
    case 'Subtraction': {
      const d = defaults as SubtractionConfig
      return {
        ...base,
        allowBorrow: bool(raw.allowBorrow, d.allowBorrow),
        allowNegativeResult: bool(raw.allowNegativeResult, d.allowNegativeResult)
      }
    }
    case 'NumberSequence': {
      const d = defaults as NumberSequenceConfig
      return {
        ...base,
        sequenceStep: num(raw.sequenceStep, d.sequenceStep)
      }
    }
    case 'Multiplication': {
      const d = defaults as MultiplicationConfig
      return {
        ...base,
        multipliers: parseNumberArray(raw.multipliers, d.multipliers)
      }
    }
    case 'FractionsBasic': {
      const d = defaults as FractionsBasicConfig
      return {
        ...base,
        fractions: parseStringArray(raw.fractions, d.fractions)
      }
    }
    default:
      return base
  }
}

function staticToJson(staticExercises: StaticExerciseConfig[]) {
  return staticExercises.map((ex) => {
    const item: Record<string, unknown> = {
      questionTextCs: ex.questionTextCs,
      questionTextEn: ex.questionTextEn,
      correctAnswer: ex.correctAnswer
    }
    if (ex.questionData && Object.keys(ex.questionData).length > 0) {
      item.questionData = ex.questionData
    }
    return item
  })
}

export function serializeTaskConfig(taskType: TaskType, config: AnyTaskConfig): string {
  const base: Record<string, unknown> = {
    minNumber: config.minNumber,
    maxNumber: config.maxNumber,
    staticExerciseChancePercent: config.staticExerciseChancePercent,
    staticExercises: staticToJson(config.staticExercises)
  }

  const typed = config as TaskConfigMap[typeof taskType]

  switch (taskType) {
    case 'Addition': {
      const c = typed as AdditionConfig
      base.allowCarry = c.allowCarry
      base.allowNegativeResult = c.allowNegativeResult
      break
    }
    case 'Subtraction': {
      const c = typed as SubtractionConfig
      base.allowBorrow = c.allowBorrow
      base.allowNegativeResult = c.allowNegativeResult
      break
    }
    case 'NumberSequence': {
      const c = typed as NumberSequenceConfig
      base.sequenceStep = c.sequenceStep
      break
    }
    case 'Multiplication': {
      const c = typed as MultiplicationConfig
      base.multipliers = c.multipliers
      break
    }
    case 'FractionsBasic': {
      const c = typed as FractionsBasicConfig
      base.fractions = c.fractions
      break
    }
  }

  return JSON.stringify(base, null, 2)
}
