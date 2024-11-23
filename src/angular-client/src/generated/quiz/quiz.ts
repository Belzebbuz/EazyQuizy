// Code generated by protoc-gen-ts_proto. DO NOT EDIT.
// versions:
//   protoc-gen-ts_proto  v2.2.5
//   protoc               v3.20.3
// source: quiz/quiz.proto

/* eslint-disable */
import { BinaryReader, BinaryWriter } from "@bufbuild/protobuf/wire";
import { type CallContext, type CallOptions } from "nice-grpc-common";

export const protobufPackage = "";

export interface CreateQuizRequest {
  name: string;
}

export interface CreateQuizResponse {
  id: string;
}

export interface AddQuestionRequest {
  id: string;
  singleAnswerQuestion: AddSingleQuestionRequest | undefined;
  multipleAnswerQuestion: AddMultipleQuestionRequest | undefined;
}

export interface GetQuizInfoRequest {
  id: string;
}

export interface GetQuizInfoResponse {
  id: string;
  name: string;
  questions: string[];
}

export interface AddSingleQuestionRequest {
  quizId: string;
  text: string;
  correctAnswer: string;
  wrongAnswers: string[];
}

export interface AddMultipleQuestionRequest {
  quizId: string;
  text: string;
  correctAnswers: string[];
  wrongAnswers: string[];
}

export interface StatusResponse {
}

function createBaseCreateQuizRequest(): CreateQuizRequest {
  return { name: "" };
}

export const CreateQuizRequest: MessageFns<CreateQuizRequest> = {
  encode(message: CreateQuizRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.name !== "") {
      writer.uint32(10).string(message.name);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): CreateQuizRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseCreateQuizRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.name = reader.string();
          continue;
        }
      }
      if ((tag & 7) === 4 || tag === 0) {
        break;
      }
      reader.skip(tag & 7);
    }
    return message;
  },

  create(base?: DeepPartial<CreateQuizRequest>): CreateQuizRequest {
    return CreateQuizRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<CreateQuizRequest>): CreateQuizRequest {
    const message = createBaseCreateQuizRequest();
    message.name = object.name ?? "";
    return message;
  },
};

function createBaseCreateQuizResponse(): CreateQuizResponse {
  return { id: "" };
}

export const CreateQuizResponse: MessageFns<CreateQuizResponse> = {
  encode(message: CreateQuizResponse, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.id !== "") {
      writer.uint32(10).string(message.id);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): CreateQuizResponse {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseCreateQuizResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.id = reader.string();
          continue;
        }
      }
      if ((tag & 7) === 4 || tag === 0) {
        break;
      }
      reader.skip(tag & 7);
    }
    return message;
  },

  create(base?: DeepPartial<CreateQuizResponse>): CreateQuizResponse {
    return CreateQuizResponse.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<CreateQuizResponse>): CreateQuizResponse {
    const message = createBaseCreateQuizResponse();
    message.id = object.id ?? "";
    return message;
  },
};

function createBaseAddQuestionRequest(): AddQuestionRequest {
  return { id: "", singleAnswerQuestion: undefined, multipleAnswerQuestion: undefined };
}

export const AddQuestionRequest: MessageFns<AddQuestionRequest> = {
  encode(message: AddQuestionRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.id !== "") {
      writer.uint32(10).string(message.id);
    }
    if (message.singleAnswerQuestion !== undefined) {
      AddSingleQuestionRequest.encode(message.singleAnswerQuestion, writer.uint32(18).fork()).join();
    }
    if (message.multipleAnswerQuestion !== undefined) {
      AddMultipleQuestionRequest.encode(message.multipleAnswerQuestion, writer.uint32(26).fork()).join();
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): AddQuestionRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAddQuestionRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.id = reader.string();
          continue;
        }
        case 2: {
          if (tag !== 18) {
            break;
          }

          message.singleAnswerQuestion = AddSingleQuestionRequest.decode(reader, reader.uint32());
          continue;
        }
        case 3: {
          if (tag !== 26) {
            break;
          }

          message.multipleAnswerQuestion = AddMultipleQuestionRequest.decode(reader, reader.uint32());
          continue;
        }
      }
      if ((tag & 7) === 4 || tag === 0) {
        break;
      }
      reader.skip(tag & 7);
    }
    return message;
  },

  create(base?: DeepPartial<AddQuestionRequest>): AddQuestionRequest {
    return AddQuestionRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<AddQuestionRequest>): AddQuestionRequest {
    const message = createBaseAddQuestionRequest();
    message.id = object.id ?? "";
    message.singleAnswerQuestion = (object.singleAnswerQuestion !== undefined && object.singleAnswerQuestion !== null)
      ? AddSingleQuestionRequest.fromPartial(object.singleAnswerQuestion)
      : undefined;
    message.multipleAnswerQuestion =
      (object.multipleAnswerQuestion !== undefined && object.multipleAnswerQuestion !== null)
        ? AddMultipleQuestionRequest.fromPartial(object.multipleAnswerQuestion)
        : undefined;
    return message;
  },
};

function createBaseGetQuizInfoRequest(): GetQuizInfoRequest {
  return { id: "" };
}

export const GetQuizInfoRequest: MessageFns<GetQuizInfoRequest> = {
  encode(message: GetQuizInfoRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.id !== "") {
      writer.uint32(10).string(message.id);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): GetQuizInfoRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGetQuizInfoRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.id = reader.string();
          continue;
        }
      }
      if ((tag & 7) === 4 || tag === 0) {
        break;
      }
      reader.skip(tag & 7);
    }
    return message;
  },

  create(base?: DeepPartial<GetQuizInfoRequest>): GetQuizInfoRequest {
    return GetQuizInfoRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<GetQuizInfoRequest>): GetQuizInfoRequest {
    const message = createBaseGetQuizInfoRequest();
    message.id = object.id ?? "";
    return message;
  },
};

function createBaseGetQuizInfoResponse(): GetQuizInfoResponse {
  return { id: "", name: "", questions: [] };
}

export const GetQuizInfoResponse: MessageFns<GetQuizInfoResponse> = {
  encode(message: GetQuizInfoResponse, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.id !== "") {
      writer.uint32(10).string(message.id);
    }
    if (message.name !== "") {
      writer.uint32(18).string(message.name);
    }
    for (const v of message.questions) {
      writer.uint32(26).string(v!);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): GetQuizInfoResponse {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGetQuizInfoResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.id = reader.string();
          continue;
        }
        case 2: {
          if (tag !== 18) {
            break;
          }

          message.name = reader.string();
          continue;
        }
        case 3: {
          if (tag !== 26) {
            break;
          }

          message.questions.push(reader.string());
          continue;
        }
      }
      if ((tag & 7) === 4 || tag === 0) {
        break;
      }
      reader.skip(tag & 7);
    }
    return message;
  },

  create(base?: DeepPartial<GetQuizInfoResponse>): GetQuizInfoResponse {
    return GetQuizInfoResponse.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<GetQuizInfoResponse>): GetQuizInfoResponse {
    const message = createBaseGetQuizInfoResponse();
    message.id = object.id ?? "";
    message.name = object.name ?? "";
    message.questions = object.questions?.map((e) => e) || [];
    return message;
  },
};

function createBaseAddSingleQuestionRequest(): AddSingleQuestionRequest {
  return { quizId: "", text: "", correctAnswer: "", wrongAnswers: [] };
}

export const AddSingleQuestionRequest: MessageFns<AddSingleQuestionRequest> = {
  encode(message: AddSingleQuestionRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.quizId !== "") {
      writer.uint32(10).string(message.quizId);
    }
    if (message.text !== "") {
      writer.uint32(18).string(message.text);
    }
    if (message.correctAnswer !== "") {
      writer.uint32(26).string(message.correctAnswer);
    }
    for (const v of message.wrongAnswers) {
      writer.uint32(34).string(v!);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): AddSingleQuestionRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAddSingleQuestionRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.quizId = reader.string();
          continue;
        }
        case 2: {
          if (tag !== 18) {
            break;
          }

          message.text = reader.string();
          continue;
        }
        case 3: {
          if (tag !== 26) {
            break;
          }

          message.correctAnswer = reader.string();
          continue;
        }
        case 4: {
          if (tag !== 34) {
            break;
          }

          message.wrongAnswers.push(reader.string());
          continue;
        }
      }
      if ((tag & 7) === 4 || tag === 0) {
        break;
      }
      reader.skip(tag & 7);
    }
    return message;
  },

  create(base?: DeepPartial<AddSingleQuestionRequest>): AddSingleQuestionRequest {
    return AddSingleQuestionRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<AddSingleQuestionRequest>): AddSingleQuestionRequest {
    const message = createBaseAddSingleQuestionRequest();
    message.quizId = object.quizId ?? "";
    message.text = object.text ?? "";
    message.correctAnswer = object.correctAnswer ?? "";
    message.wrongAnswers = object.wrongAnswers?.map((e) => e) || [];
    return message;
  },
};

function createBaseAddMultipleQuestionRequest(): AddMultipleQuestionRequest {
  return { quizId: "", text: "", correctAnswers: [], wrongAnswers: [] };
}

export const AddMultipleQuestionRequest: MessageFns<AddMultipleQuestionRequest> = {
  encode(message: AddMultipleQuestionRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.quizId !== "") {
      writer.uint32(10).string(message.quizId);
    }
    if (message.text !== "") {
      writer.uint32(18).string(message.text);
    }
    for (const v of message.correctAnswers) {
      writer.uint32(26).string(v!);
    }
    for (const v of message.wrongAnswers) {
      writer.uint32(34).string(v!);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): AddMultipleQuestionRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseAddMultipleQuestionRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.quizId = reader.string();
          continue;
        }
        case 2: {
          if (tag !== 18) {
            break;
          }

          message.text = reader.string();
          continue;
        }
        case 3: {
          if (tag !== 26) {
            break;
          }

          message.correctAnswers.push(reader.string());
          continue;
        }
        case 4: {
          if (tag !== 34) {
            break;
          }

          message.wrongAnswers.push(reader.string());
          continue;
        }
      }
      if ((tag & 7) === 4 || tag === 0) {
        break;
      }
      reader.skip(tag & 7);
    }
    return message;
  },

  create(base?: DeepPartial<AddMultipleQuestionRequest>): AddMultipleQuestionRequest {
    return AddMultipleQuestionRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<AddMultipleQuestionRequest>): AddMultipleQuestionRequest {
    const message = createBaseAddMultipleQuestionRequest();
    message.quizId = object.quizId ?? "";
    message.text = object.text ?? "";
    message.correctAnswers = object.correctAnswers?.map((e) => e) || [];
    message.wrongAnswers = object.wrongAnswers?.map((e) => e) || [];
    return message;
  },
};

function createBaseStatusResponse(): StatusResponse {
  return {};
}

export const StatusResponse: MessageFns<StatusResponse> = {
  encode(_: StatusResponse, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): StatusResponse {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseStatusResponse();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
      }
      if ((tag & 7) === 4 || tag === 0) {
        break;
      }
      reader.skip(tag & 7);
    }
    return message;
  },

  create(base?: DeepPartial<StatusResponse>): StatusResponse {
    return StatusResponse.fromPartial(base ?? {});
  },
  fromPartial(_: DeepPartial<StatusResponse>): StatusResponse {
    const message = createBaseStatusResponse();
    return message;
  },
};

export type QuizServiceDefinition = typeof QuizServiceDefinition;
export const QuizServiceDefinition = {
  name: "QuizService",
  fullName: "QuizService",
  methods: {
    create: {
      name: "Create",
      requestType: CreateQuizRequest,
      requestStream: false,
      responseType: CreateQuizResponse,
      responseStream: false,
      options: {
        _unknownFields: { 578365826: [new Uint8Array([13, 34, 8, 47, 118, 49, 47, 113, 117, 105, 122, 58, 1, 42])] },
      },
    },
    addSingleQuestion: {
      name: "AddSingleQuestion",
      requestType: AddSingleQuestionRequest,
      requestStream: false,
      responseType: StatusResponse,
      responseStream: false,
      options: {
        _unknownFields: {
          578365826: [
            new Uint8Array([
              20,
              34,
              15,
              47,
              118,
              49,
              47,
              113,
              117,
              105,
              122,
              47,
              115,
              105,
              110,
              103,
              108,
              101,
              58,
              1,
              42,
            ]),
          ],
        },
      },
    },
    addMultipleQuestion: {
      name: "AddMultipleQuestion",
      requestType: AddMultipleQuestionRequest,
      requestStream: false,
      responseType: StatusResponse,
      responseStream: false,
      options: {
        _unknownFields: {
          578365826: [
            new Uint8Array([
              22,
              34,
              17,
              47,
              118,
              49,
              47,
              113,
              117,
              105,
              122,
              47,
              109,
              117,
              108,
              116,
              105,
              112,
              108,
              101,
              58,
              1,
              42,
            ]),
          ],
        },
      },
    },
    getInfo: {
      name: "GetInfo",
      requestType: GetQuizInfoRequest,
      requestStream: false,
      responseType: GetQuizInfoResponse,
      responseStream: false,
      options: {
        _unknownFields: {
          578365826: [new Uint8Array([15, 18, 13, 47, 118, 49, 47, 113, 117, 105, 122, 47, 123, 105, 100, 125])],
        },
      },
    },
  },
} as const;

export interface QuizServiceImplementation<CallContextExt = {}> {
  create(request: CreateQuizRequest, context: CallContext & CallContextExt): Promise<DeepPartial<CreateQuizResponse>>;
  addSingleQuestion(
    request: AddSingleQuestionRequest,
    context: CallContext & CallContextExt,
  ): Promise<DeepPartial<StatusResponse>>;
  addMultipleQuestion(
    request: AddMultipleQuestionRequest,
    context: CallContext & CallContextExt,
  ): Promise<DeepPartial<StatusResponse>>;
  getInfo(
    request: GetQuizInfoRequest,
    context: CallContext & CallContextExt,
  ): Promise<DeepPartial<GetQuizInfoResponse>>;
}

export interface QuizServiceClient<CallOptionsExt = {}> {
  create(request: DeepPartial<CreateQuizRequest>, options?: CallOptions & CallOptionsExt): Promise<CreateQuizResponse>;
  addSingleQuestion(
    request: DeepPartial<AddSingleQuestionRequest>,
    options?: CallOptions & CallOptionsExt,
  ): Promise<StatusResponse>;
  addMultipleQuestion(
    request: DeepPartial<AddMultipleQuestionRequest>,
    options?: CallOptions & CallOptionsExt,
  ): Promise<StatusResponse>;
  getInfo(
    request: DeepPartial<GetQuizInfoRequest>,
    options?: CallOptions & CallOptionsExt,
  ): Promise<GetQuizInfoResponse>;
}

type Builtin = Date | Function | Uint8Array | string | number | boolean | undefined;

export type DeepPartial<T> = T extends Builtin ? T
  : T extends globalThis.Array<infer U> ? globalThis.Array<DeepPartial<U>>
  : T extends ReadonlyArray<infer U> ? ReadonlyArray<DeepPartial<U>>
  : T extends {} ? { [K in keyof T]?: DeepPartial<T[K]> }
  : Partial<T>;

export interface MessageFns<T> {
  encode(message: T, writer?: BinaryWriter): BinaryWriter;
  decode(input: BinaryReader | Uint8Array, length?: number): T;
  create(base?: DeepPartial<T>): T;
  fromPartial(object: DeepPartial<T>): T;
}
