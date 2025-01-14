// Code generated by protoc-gen-ts_proto. DO NOT EDIT.
// versions:
//   protoc-gen-ts_proto  v2.2.5
//   protoc               v3.20.3
// source: quiz/lobby.proto

/* eslint-disable */
import { BinaryReader, BinaryWriter } from "@bufbuild/protobuf/wire";
import { type CallContext, type CallOptions } from "nice-grpc-common";
import { StringValue } from "../google/protobuf/wrappers";
import { StatusResponse } from "../types/types";

export const protobufPackage = "";

export enum LobbyState {
  created = 0,
  started = 1,
  closed = 2,
  UNRECOGNIZED = -1,
}

export interface CreateGameRequest {
  lobbyId: string;
}

export interface GetLobbyInfoRequest {
  id: string;
}

export interface GetLobbyInfoResponse {
  id: string;
  quizInfo: LobbyQuizInfo | undefined;
  updateChannel: string;
  settings: LobbySettings | undefined;
  state: LobbyState;
  canInvite: boolean;
  players: PlayerInfo[];
  gameId: string | undefined;
}

export interface LobbyQuizInfo {
  id: string;
  name: string;
}

export interface CreateLobbyRequest {
  quizId: string;
  settings: LobbySettings | undefined;
}

export interface UpdateLobbySettingsRequest {
  lobbyId: string;
  settings: LobbySettings | undefined;
}

export interface ConnectPlayerRequest {
  lobbyId: string;
  playerName: string;
}

export interface DisconnectPlayerRequest {
  lobbyId: string;
}

export interface InvitePlayerRequest {
  lobbyId: string;
  playerId: string;
}

export interface RemovePlayerRequest {
  lobbyId: string;
  playerId: string;
}

export interface LobbySettings {
  timePerQuestion: number;
  isOpen: boolean;
  maxPlayersCount: number;
}

export interface PlayerInfo {
  id: string;
  name: string;
  isOwner: boolean;
  invited: boolean;
}

function createBaseCreateGameRequest(): CreateGameRequest {
  return { lobbyId: "" };
}

export const CreateGameRequest: MessageFns<CreateGameRequest> = {
  encode(message: CreateGameRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.lobbyId !== "") {
      writer.uint32(10).string(message.lobbyId);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): CreateGameRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseCreateGameRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.lobbyId = reader.string();
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

  create(base?: DeepPartial<CreateGameRequest>): CreateGameRequest {
    return CreateGameRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<CreateGameRequest>): CreateGameRequest {
    const message = createBaseCreateGameRequest();
    message.lobbyId = object.lobbyId ?? "";
    return message;
  },
};

function createBaseGetLobbyInfoRequest(): GetLobbyInfoRequest {
  return { id: "" };
}

export const GetLobbyInfoRequest: MessageFns<GetLobbyInfoRequest> = {
  encode(message: GetLobbyInfoRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.id !== "") {
      writer.uint32(10).string(message.id);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): GetLobbyInfoRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGetLobbyInfoRequest();
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

  create(base?: DeepPartial<GetLobbyInfoRequest>): GetLobbyInfoRequest {
    return GetLobbyInfoRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<GetLobbyInfoRequest>): GetLobbyInfoRequest {
    const message = createBaseGetLobbyInfoRequest();
    message.id = object.id ?? "";
    return message;
  },
};

function createBaseGetLobbyInfoResponse(): GetLobbyInfoResponse {
  return {
    id: "",
    quizInfo: undefined,
    updateChannel: "",
    settings: undefined,
    state: 0,
    canInvite: false,
    players: [],
    gameId: undefined,
  };
}

export const GetLobbyInfoResponse: MessageFns<GetLobbyInfoResponse> = {
  encode(message: GetLobbyInfoResponse, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.id !== "") {
      writer.uint32(10).string(message.id);
    }
    if (message.quizInfo !== undefined) {
      LobbyQuizInfo.encode(message.quizInfo, writer.uint32(18).fork()).join();
    }
    if (message.updateChannel !== "") {
      writer.uint32(26).string(message.updateChannel);
    }
    if (message.settings !== undefined) {
      LobbySettings.encode(message.settings, writer.uint32(34).fork()).join();
    }
    if (message.state !== 0) {
      writer.uint32(40).int32(message.state);
    }
    if (message.canInvite !== false) {
      writer.uint32(48).bool(message.canInvite);
    }
    for (const v of message.players) {
      PlayerInfo.encode(v!, writer.uint32(58).fork()).join();
    }
    if (message.gameId !== undefined) {
      StringValue.encode({ value: message.gameId! }, writer.uint32(66).fork()).join();
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): GetLobbyInfoResponse {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseGetLobbyInfoResponse();
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

          message.quizInfo = LobbyQuizInfo.decode(reader, reader.uint32());
          continue;
        }
        case 3: {
          if (tag !== 26) {
            break;
          }

          message.updateChannel = reader.string();
          continue;
        }
        case 4: {
          if (tag !== 34) {
            break;
          }

          message.settings = LobbySettings.decode(reader, reader.uint32());
          continue;
        }
        case 5: {
          if (tag !== 40) {
            break;
          }

          message.state = reader.int32() as any;
          continue;
        }
        case 6: {
          if (tag !== 48) {
            break;
          }

          message.canInvite = reader.bool();
          continue;
        }
        case 7: {
          if (tag !== 58) {
            break;
          }

          message.players.push(PlayerInfo.decode(reader, reader.uint32()));
          continue;
        }
        case 8: {
          if (tag !== 66) {
            break;
          }

          message.gameId = StringValue.decode(reader, reader.uint32()).value;
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

  create(base?: DeepPartial<GetLobbyInfoResponse>): GetLobbyInfoResponse {
    return GetLobbyInfoResponse.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<GetLobbyInfoResponse>): GetLobbyInfoResponse {
    const message = createBaseGetLobbyInfoResponse();
    message.id = object.id ?? "";
    message.quizInfo = (object.quizInfo !== undefined && object.quizInfo !== null)
      ? LobbyQuizInfo.fromPartial(object.quizInfo)
      : undefined;
    message.updateChannel = object.updateChannel ?? "";
    message.settings = (object.settings !== undefined && object.settings !== null)
      ? LobbySettings.fromPartial(object.settings)
      : undefined;
    message.state = object.state ?? 0;
    message.canInvite = object.canInvite ?? false;
    message.players = object.players?.map((e) => PlayerInfo.fromPartial(e)) || [];
    message.gameId = object.gameId ?? undefined;
    return message;
  },
};

function createBaseLobbyQuizInfo(): LobbyQuizInfo {
  return { id: "", name: "" };
}

export const LobbyQuizInfo: MessageFns<LobbyQuizInfo> = {
  encode(message: LobbyQuizInfo, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.id !== "") {
      writer.uint32(10).string(message.id);
    }
    if (message.name !== "") {
      writer.uint32(18).string(message.name);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): LobbyQuizInfo {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseLobbyQuizInfo();
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
      }
      if ((tag & 7) === 4 || tag === 0) {
        break;
      }
      reader.skip(tag & 7);
    }
    return message;
  },

  create(base?: DeepPartial<LobbyQuizInfo>): LobbyQuizInfo {
    return LobbyQuizInfo.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<LobbyQuizInfo>): LobbyQuizInfo {
    const message = createBaseLobbyQuizInfo();
    message.id = object.id ?? "";
    message.name = object.name ?? "";
    return message;
  },
};

function createBaseCreateLobbyRequest(): CreateLobbyRequest {
  return { quizId: "", settings: undefined };
}

export const CreateLobbyRequest: MessageFns<CreateLobbyRequest> = {
  encode(message: CreateLobbyRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.quizId !== "") {
      writer.uint32(10).string(message.quizId);
    }
    if (message.settings !== undefined) {
      LobbySettings.encode(message.settings, writer.uint32(18).fork()).join();
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): CreateLobbyRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseCreateLobbyRequest();
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

          message.settings = LobbySettings.decode(reader, reader.uint32());
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

  create(base?: DeepPartial<CreateLobbyRequest>): CreateLobbyRequest {
    return CreateLobbyRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<CreateLobbyRequest>): CreateLobbyRequest {
    const message = createBaseCreateLobbyRequest();
    message.quizId = object.quizId ?? "";
    message.settings = (object.settings !== undefined && object.settings !== null)
      ? LobbySettings.fromPartial(object.settings)
      : undefined;
    return message;
  },
};

function createBaseUpdateLobbySettingsRequest(): UpdateLobbySettingsRequest {
  return { lobbyId: "", settings: undefined };
}

export const UpdateLobbySettingsRequest: MessageFns<UpdateLobbySettingsRequest> = {
  encode(message: UpdateLobbySettingsRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.lobbyId !== "") {
      writer.uint32(10).string(message.lobbyId);
    }
    if (message.settings !== undefined) {
      LobbySettings.encode(message.settings, writer.uint32(18).fork()).join();
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): UpdateLobbySettingsRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseUpdateLobbySettingsRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.lobbyId = reader.string();
          continue;
        }
        case 2: {
          if (tag !== 18) {
            break;
          }

          message.settings = LobbySettings.decode(reader, reader.uint32());
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

  create(base?: DeepPartial<UpdateLobbySettingsRequest>): UpdateLobbySettingsRequest {
    return UpdateLobbySettingsRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<UpdateLobbySettingsRequest>): UpdateLobbySettingsRequest {
    const message = createBaseUpdateLobbySettingsRequest();
    message.lobbyId = object.lobbyId ?? "";
    message.settings = (object.settings !== undefined && object.settings !== null)
      ? LobbySettings.fromPartial(object.settings)
      : undefined;
    return message;
  },
};

function createBaseConnectPlayerRequest(): ConnectPlayerRequest {
  return { lobbyId: "", playerName: "" };
}

export const ConnectPlayerRequest: MessageFns<ConnectPlayerRequest> = {
  encode(message: ConnectPlayerRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.lobbyId !== "") {
      writer.uint32(10).string(message.lobbyId);
    }
    if (message.playerName !== "") {
      writer.uint32(18).string(message.playerName);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): ConnectPlayerRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseConnectPlayerRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.lobbyId = reader.string();
          continue;
        }
        case 2: {
          if (tag !== 18) {
            break;
          }

          message.playerName = reader.string();
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

  create(base?: DeepPartial<ConnectPlayerRequest>): ConnectPlayerRequest {
    return ConnectPlayerRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<ConnectPlayerRequest>): ConnectPlayerRequest {
    const message = createBaseConnectPlayerRequest();
    message.lobbyId = object.lobbyId ?? "";
    message.playerName = object.playerName ?? "";
    return message;
  },
};

function createBaseDisconnectPlayerRequest(): DisconnectPlayerRequest {
  return { lobbyId: "" };
}

export const DisconnectPlayerRequest: MessageFns<DisconnectPlayerRequest> = {
  encode(message: DisconnectPlayerRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.lobbyId !== "") {
      writer.uint32(10).string(message.lobbyId);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): DisconnectPlayerRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseDisconnectPlayerRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.lobbyId = reader.string();
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

  create(base?: DeepPartial<DisconnectPlayerRequest>): DisconnectPlayerRequest {
    return DisconnectPlayerRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<DisconnectPlayerRequest>): DisconnectPlayerRequest {
    const message = createBaseDisconnectPlayerRequest();
    message.lobbyId = object.lobbyId ?? "";
    return message;
  },
};

function createBaseInvitePlayerRequest(): InvitePlayerRequest {
  return { lobbyId: "", playerId: "" };
}

export const InvitePlayerRequest: MessageFns<InvitePlayerRequest> = {
  encode(message: InvitePlayerRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.lobbyId !== "") {
      writer.uint32(10).string(message.lobbyId);
    }
    if (message.playerId !== "") {
      writer.uint32(18).string(message.playerId);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): InvitePlayerRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseInvitePlayerRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.lobbyId = reader.string();
          continue;
        }
        case 2: {
          if (tag !== 18) {
            break;
          }

          message.playerId = reader.string();
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

  create(base?: DeepPartial<InvitePlayerRequest>): InvitePlayerRequest {
    return InvitePlayerRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<InvitePlayerRequest>): InvitePlayerRequest {
    const message = createBaseInvitePlayerRequest();
    message.lobbyId = object.lobbyId ?? "";
    message.playerId = object.playerId ?? "";
    return message;
  },
};

function createBaseRemovePlayerRequest(): RemovePlayerRequest {
  return { lobbyId: "", playerId: "" };
}

export const RemovePlayerRequest: MessageFns<RemovePlayerRequest> = {
  encode(message: RemovePlayerRequest, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.lobbyId !== "") {
      writer.uint32(10).string(message.lobbyId);
    }
    if (message.playerId !== "") {
      writer.uint32(18).string(message.playerId);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): RemovePlayerRequest {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseRemovePlayerRequest();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 10) {
            break;
          }

          message.lobbyId = reader.string();
          continue;
        }
        case 2: {
          if (tag !== 18) {
            break;
          }

          message.playerId = reader.string();
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

  create(base?: DeepPartial<RemovePlayerRequest>): RemovePlayerRequest {
    return RemovePlayerRequest.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<RemovePlayerRequest>): RemovePlayerRequest {
    const message = createBaseRemovePlayerRequest();
    message.lobbyId = object.lobbyId ?? "";
    message.playerId = object.playerId ?? "";
    return message;
  },
};

function createBaseLobbySettings(): LobbySettings {
  return { timePerQuestion: 0, isOpen: false, maxPlayersCount: 0 };
}

export const LobbySettings: MessageFns<LobbySettings> = {
  encode(message: LobbySettings, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.timePerQuestion !== 0) {
      writer.uint32(8).int32(message.timePerQuestion);
    }
    if (message.isOpen !== false) {
      writer.uint32(16).bool(message.isOpen);
    }
    if (message.maxPlayersCount !== 0) {
      writer.uint32(24).int32(message.maxPlayersCount);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): LobbySettings {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBaseLobbySettings();
    while (reader.pos < end) {
      const tag = reader.uint32();
      switch (tag >>> 3) {
        case 1: {
          if (tag !== 8) {
            break;
          }

          message.timePerQuestion = reader.int32();
          continue;
        }
        case 2: {
          if (tag !== 16) {
            break;
          }

          message.isOpen = reader.bool();
          continue;
        }
        case 3: {
          if (tag !== 24) {
            break;
          }

          message.maxPlayersCount = reader.int32();
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

  create(base?: DeepPartial<LobbySettings>): LobbySettings {
    return LobbySettings.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<LobbySettings>): LobbySettings {
    const message = createBaseLobbySettings();
    message.timePerQuestion = object.timePerQuestion ?? 0;
    message.isOpen = object.isOpen ?? false;
    message.maxPlayersCount = object.maxPlayersCount ?? 0;
    return message;
  },
};

function createBasePlayerInfo(): PlayerInfo {
  return { id: "", name: "", isOwner: false, invited: false };
}

export const PlayerInfo: MessageFns<PlayerInfo> = {
  encode(message: PlayerInfo, writer: BinaryWriter = new BinaryWriter()): BinaryWriter {
    if (message.id !== "") {
      writer.uint32(10).string(message.id);
    }
    if (message.name !== "") {
      writer.uint32(18).string(message.name);
    }
    if (message.isOwner !== false) {
      writer.uint32(24).bool(message.isOwner);
    }
    if (message.invited !== false) {
      writer.uint32(32).bool(message.invited);
    }
    return writer;
  },

  decode(input: BinaryReader | Uint8Array, length?: number): PlayerInfo {
    const reader = input instanceof BinaryReader ? input : new BinaryReader(input);
    let end = length === undefined ? reader.len : reader.pos + length;
    const message = createBasePlayerInfo();
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
          if (tag !== 24) {
            break;
          }

          message.isOwner = reader.bool();
          continue;
        }
        case 4: {
          if (tag !== 32) {
            break;
          }

          message.invited = reader.bool();
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

  create(base?: DeepPartial<PlayerInfo>): PlayerInfo {
    return PlayerInfo.fromPartial(base ?? {});
  },
  fromPartial(object: DeepPartial<PlayerInfo>): PlayerInfo {
    const message = createBasePlayerInfo();
    message.id = object.id ?? "";
    message.name = object.name ?? "";
    message.isOwner = object.isOwner ?? false;
    message.invited = object.invited ?? false;
    return message;
  },
};

export type LobbyServiceDefinition = typeof LobbyServiceDefinition;
export const LobbyServiceDefinition = {
  name: "LobbyService",
  fullName: "LobbyService",
  methods: {
    create: {
      name: "Create",
      requestType: CreateLobbyRequest,
      requestStream: false,
      responseType: StatusResponse,
      responseStream: false,
      options: {},
    },
    getInfo: {
      name: "GetInfo",
      requestType: GetLobbyInfoRequest,
      requestStream: false,
      responseType: GetLobbyInfoResponse,
      responseStream: false,
      options: {},
    },
    updateSettings: {
      name: "UpdateSettings",
      requestType: UpdateLobbySettingsRequest,
      requestStream: false,
      responseType: StatusResponse,
      responseStream: false,
      options: {},
    },
    connectPlayer: {
      name: "ConnectPlayer",
      requestType: ConnectPlayerRequest,
      requestStream: false,
      responseType: StatusResponse,
      responseStream: false,
      options: {},
    },
    disconnectPlayer: {
      name: "DisconnectPlayer",
      requestType: DisconnectPlayerRequest,
      requestStream: false,
      responseType: StatusResponse,
      responseStream: false,
      options: {},
    },
    invitePlayer: {
      name: "InvitePlayer",
      requestType: InvitePlayerRequest,
      requestStream: false,
      responseType: StatusResponse,
      responseStream: false,
      options: {},
    },
    removePlayer: {
      name: "RemovePlayer",
      requestType: RemovePlayerRequest,
      requestStream: false,
      responseType: StatusResponse,
      responseStream: false,
      options: {},
    },
    startGame: {
      name: "StartGame",
      requestType: CreateGameRequest,
      requestStream: false,
      responseType: StatusResponse,
      responseStream: false,
      options: {},
    },
  },
} as const;

export interface LobbyServiceImplementation<CallContextExt = {}> {
  create(request: CreateLobbyRequest, context: CallContext & CallContextExt): Promise<DeepPartial<StatusResponse>>;
  getInfo(
    request: GetLobbyInfoRequest,
    context: CallContext & CallContextExt,
  ): Promise<DeepPartial<GetLobbyInfoResponse>>;
  updateSettings(
    request: UpdateLobbySettingsRequest,
    context: CallContext & CallContextExt,
  ): Promise<DeepPartial<StatusResponse>>;
  connectPlayer(
    request: ConnectPlayerRequest,
    context: CallContext & CallContextExt,
  ): Promise<DeepPartial<StatusResponse>>;
  disconnectPlayer(
    request: DisconnectPlayerRequest,
    context: CallContext & CallContextExt,
  ): Promise<DeepPartial<StatusResponse>>;
  invitePlayer(
    request: InvitePlayerRequest,
    context: CallContext & CallContextExt,
  ): Promise<DeepPartial<StatusResponse>>;
  removePlayer(
    request: RemovePlayerRequest,
    context: CallContext & CallContextExt,
  ): Promise<DeepPartial<StatusResponse>>;
  startGame(request: CreateGameRequest, context: CallContext & CallContextExt): Promise<DeepPartial<StatusResponse>>;
}

export interface LobbyServiceClient<CallOptionsExt = {}> {
  create(request: DeepPartial<CreateLobbyRequest>, options?: CallOptions & CallOptionsExt): Promise<StatusResponse>;
  getInfo(
    request: DeepPartial<GetLobbyInfoRequest>,
    options?: CallOptions & CallOptionsExt,
  ): Promise<GetLobbyInfoResponse>;
  updateSettings(
    request: DeepPartial<UpdateLobbySettingsRequest>,
    options?: CallOptions & CallOptionsExt,
  ): Promise<StatusResponse>;
  connectPlayer(
    request: DeepPartial<ConnectPlayerRequest>,
    options?: CallOptions & CallOptionsExt,
  ): Promise<StatusResponse>;
  disconnectPlayer(
    request: DeepPartial<DisconnectPlayerRequest>,
    options?: CallOptions & CallOptionsExt,
  ): Promise<StatusResponse>;
  invitePlayer(
    request: DeepPartial<InvitePlayerRequest>,
    options?: CallOptions & CallOptionsExt,
  ): Promise<StatusResponse>;
  removePlayer(
    request: DeepPartial<RemovePlayerRequest>,
    options?: CallOptions & CallOptionsExt,
  ): Promise<StatusResponse>;
  startGame(request: DeepPartial<CreateGameRequest>, options?: CallOptions & CallOptionsExt): Promise<StatusResponse>;
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
