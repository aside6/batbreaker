function CheckAttrCost() {
    $('.AttrAmount').each(function () {
        var Points = parseInt($('#txtPoints').val());
        var type = this.id.replace('divAmount', '');
        var Amount = parseInt($('#divAmount' + type).html());
        var Cost = (Amount * 2) + 5;
        if (Cost <= Points) {
            $('#Raise' + type).show();
        }
        else {
            $('#Raise' + type).hide();
        }
    });
}

function RaiseAttr(type) {
    var Points = parseInt($('#txtPoints').val());
    var Amount = parseInt($('#divAmount' + type).html());
    var Cost = (Amount * 2) + 5;

    if (Cost <= Points) {
        Points = Points - Cost;
        Amount = Amount + 1;

        $('#txtPoints').val(Points.toString());
        $('#graph' + type).css("width", (Amount * 2).toString() + "px");
        $('#divAmount' + type).html(Amount.toString());
        $('#' + type).val(Amount.toString());

        CheckAttrCost()
    }
    else {
        alert("You don't have enough points to raise this Attribute");
    }
}