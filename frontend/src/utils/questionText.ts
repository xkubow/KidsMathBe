const OPERATOR_SPLIT = /([+\-×÷*/=])/
const OPERATORS = new Set(['+', '-', '×', '÷', '*', '/', '='])

export interface QuestionSegment {
  text: string
  isOperator: boolean
}

export function parseQuestionSegments(text: string): QuestionSegment[] {
  return text
    .split(OPERATOR_SPLIT)
    .filter(Boolean)
    .map(part => ({
      text: part,
      isOperator: OPERATORS.has(part),
    }))
}
