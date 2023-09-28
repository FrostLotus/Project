
// MFC_TESTView.h: CMFCTESTView 類別的介面
//

#pragma once


class CMFCTESTView : public CView
{
protected: // 僅從序列化建立
	CMFCTESTView() noexcept;
	DECLARE_DYNCREATE(CMFCTESTView)

// 屬性
public:
	CMFCTESTDoc* GetDocument() const;

// 作業
public:

// 覆寫
public:
	virtual void OnDraw(CDC* pDC);  // 覆寫以描繪此檢視
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// 程式碼實作
public:
	virtual ~CMFCTESTView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// 產生的訊息對應函式
protected:
	afx_msg void OnFilePrintPreview();
	afx_msg void OnRButtonUp(UINT nFlags, CPoint point);
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG  // 對 MFC_TESTView.cpp 中的版本進行偵錯
inline CMFCTESTDoc* CMFCTESTView::GetDocument() const
   { return reinterpret_cast<CMFCTESTDoc*>(m_pDocument); }
#endif

