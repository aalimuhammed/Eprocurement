function loadmainwork(){
  
    $.ajax({
        url:'/BOQStands/boqstands',
        type:'GET',
        success:function(response){
        // console.log(response);  
         $('#mainboq').empty();
         var len = response.length;
                                
         $('#mainboq').html("<option selected=true disabled=disabled> Please choose a option </option>")
    
         for( var i = 0; i<len; i++){
             var code = response[i]['id'];
             var name = response[i]['name'];
             
            // $("#statusval").append("<option value='"+code+"'>"+name+"</option>");
            $('#mainboq').append('<option value="' + code + '">' + name + '</option>');

         } 

        },
        error: function () {
         alert(response.err);
            }
        });



}

$(document).ready(function(){
    loadmainwork();
    
    $('#mainboq').on('change', function() {
    var idvalue = $(this).val();
    
     $.ajax({
         url:'/BOQChapters/GetBOQChapters',
         type:'GET',
         data:{
              "id" : idvalue
         },
      
         success:function(response){
            
            $('#boqchapters').empty();
            var len = response.length;
                                   
            $('#boqchapters').html("<option selected=true disabled=disabled>Please choose an option </option>")
       
            for( var i = 0; i<len; i++){
                var code = response[i]['id'];
                var name = response[i]['name'];
                
               $('#boqchapters').append('<option value="' + code + '">' + name + '</option>');
    
            } 
    
         },
         error: function () {
          alert(response.err);
             }
         });
    
    
    
    });
    
    //$('#submainwork').on('change', function() {
    //     var text = $('#submainwork :selected').text();
      
    //    // var txtdata = text.replace(/'([^']+)':/g, '$1:');
      
    //     var code = $(this).val();
    
    
    //     $('#skills').append("<option selected=true value="+code+">"+text+"</option>");
         
    //    });
    
    
    
    
    });