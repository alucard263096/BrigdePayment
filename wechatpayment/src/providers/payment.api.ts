import { Injectable } from '@angular/core';
import { LoadingController, Loading} from 'ionic-angular';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Headers, RequestOptions } from '@angular/http';
import { ApiConfig } from '../app/api.config'

import 'rxjs/add/operator/toPromise';
import 'rxjs/add/operator/map';

/*
  Generated class for the Test provider.

  See https://angular.io/docs/ts/latest/guide/dependency-injection.html
  for more info on providers and Angular 2 DI.
*/
@Injectable()
export class PaymentApi {

    constructor(public http: Http) {

    }


    //获取广告列表，传入对应的搜索条件
    public list(params) {
        var url = "http://bridgepayment.app-link.org/payment.aspx";
        var headers = ApiConfig.GetHeader(url, params);
        let options = new RequestOptions({ headers: headers });
        let body = ApiConfig.ParamUrlencoded(params);

        return this.http.post(url, body, options).toPromise()
            .then((res) => {
                return res.json();
            })
            .catch(err => {
                alert("error");
            });


    }


}
