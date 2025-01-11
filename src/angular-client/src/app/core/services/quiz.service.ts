import {inject, Injectable} from '@angular/core';
import {
  AddMultipleQuestionRequest,
  AddOrderQuestionRequest,
  AddRangeQuestionRequest,
  AddSingleQuestionRequest,
  CreateQuizRequest, DeleteQuestionRequest,
  DeleteQuizRequest,
  GenerateWrongAnswersRequest,
  GetQuestionInfoRequest,
  GetQuizInfoRequest,
  GetQuizInfoResponse,
  QuizServiceClient,
  QuizServiceDefinition,
  SearchUserQuizRequest,
  SearchUserQuizResponse,
  SetQuestionNewOrderRequest
} from '../../../generated/quiz/quiz';
import {GrpcService} from './grpc.service';
import {StatusResponse} from '../../../generated/types/types';

@Injectable({
  providedIn: 'root'
})
export class QuizService {
  client: QuizServiceClient;
  constructor(private readonly grpc: GrpcService) {
    this.client = grpc.getClient(QuizServiceDefinition);
  }

  public async search(page: number, pageSize = 10) : Promise<SearchUserQuizResponse>{
    const request = SearchUserQuizRequest.create();
    request.page = page;
    request.pageSize = pageSize;
    return await this.client.searchUserQuiz(request);
  }

  public async create(request: CreateQuizRequest) : Promise<StatusResponse>{
    return await this.client.create(request);
  }
  public async addSingleQuestion(request: AddSingleQuestionRequest) : Promise<StatusResponse>{
    return await this.client.addSingleQuestion(request);
  }
  public async addMultipleQuestion(request: AddMultipleQuestionRequest) : Promise<StatusResponse>{
    return await this.client.addMultipleQuestion(request);
  }
  public async addRangeQuestion(request: AddRangeQuestionRequest) : Promise<StatusResponse>{
    return await this.client.addRangeQuestion(request);
  }
  public async addOrderQuestion(request: AddOrderQuestionRequest) : Promise<StatusResponse>{
    return await this.client.addOrderQuestion(request);
  }
  public async setQuestionNewOrder(quizId: string, questionId: string, newOrder: number) : Promise<StatusResponse>{
    const request = SetQuestionNewOrderRequest.create();
    request.quizId = quizId;
    request.questionId = questionId;
    request.newOrder = newOrder;
    return await this.client.setQuestionNewOrder(request);
  }

  public async getInfo(id: string) : Promise<GetQuizInfoResponse>{
    const request = GetQuizInfoRequest.create();
    request.id = id;
    return await this.client.getQuizInfo(request);
  }
  public async getQuestionInfo(quizId: string, questionId: string) {
    const request = GetQuestionInfoRequest.create();
    request.quizId = quizId;
    request.questionId = questionId;
    return await this.client.getQuestionInfo(request);
  }
  public async generateWrongAnswers(text: string, count: number){
    const request = GenerateWrongAnswersRequest.create();
    request.text = text;
    request.count = count;
    return await this.client.generateWrongAnswers(request);
  }

  public async delete(id: string) {
    const request = DeleteQuizRequest.create();
    request.quizId = id;
    return await this.client.deleteQuiz(request);
  }
  public async deleteQuestion(quizId: string, questionId: string) {
    const request = DeleteQuestionRequest.create();
    request.quizId = quizId;
    request.questionId = questionId;
    return await this.client.deleteQuestion(request);
  }
}
