import { Component } from '@angular/core';
import { NavController } from 'ionic-angular';
import { PaymentApi } from '../../providers/payment.api';
import { ApiConfig } from '../../app/api.config';


@Component({
  selector: 'page-home',
  templateUrl: 'home.html',
  providers: [PaymentApi]
})
export class HomePage {

    public orderno = "t001";
    public amount = "0.01";
    public customerid = "c001";
    public subject = "test pay";

    constructor(public navCtrl: NavController, public paymentApi: PaymentApi) {


  }

    public payment() {
        var sign = "abcd1234&orderno=" + this.orderno + "&customerid=" + this.customerid + "&amount=" + this.amount;
        sign = ApiConfig.MD5(sign);
        var json = { "orderno": this.orderno, "customerid": this.customerid, "amount": this.amount, "subject": this.subject, "sign": sign };
        this.paymentApi.list(json).then((data) => {
            alert(JSON.stringify(data));
            Wechat.sendPaymentRequest(data.ret, function () {
                alert("Success");
            }, function (reason) {
                alert("Failed: " + reason);
            });
        });
    }
}
